using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.DelightingInternal;
using UnityEngine;

public class DelightingCLI
{
    public static void Run()
    {
        UnityEditor.Experimental.Delighting.CLI.Run();
    }
}

namespace UnityEditor.Experimental
{
    using UnityDebug = UnityEngine.Debug;

    public static partial class Delighting
    {
        public static class CLI
        {
            struct Command
            {
                internal string baseTexturePath;
                internal string normalsTexturePath;
                internal string bentNormalsTexturePath;
                internal string ambientOcclusionTexturePath;
                internal string positionTexturePath;
                internal string maskTexturePath;

                internal string inputFolderPath;

                internal bool? switchYZ;
                internal float? separateDarkAreas;
                internal float? forceLocalDelighting;
                internal float? removeHighlights;
                internal float? removeDarkNoise;

                internal string delightedTexturePath;
            }

            public static void Run()
            {
                var args = System.Environment.GetCommandLineArgs();
                RunArgs(args);
            }

            public static void RunArgs(string[] args)
            {
                var command = Parse(args);
                Execute(command);
            }

            static void Execute(Command command)
            {
                if (string.IsNullOrEmpty(command.delightedTexturePath))
                    throw new Exception("-output must be defined");

                var service = Delighting.NewService();

                var input = new Input()
                    .SetSwitchYZ(command.switchYZ)
                    .SetForceLocalDelighting(command.forceLocalDelighting)
                    .SetSeparateDarkAreas(command.separateDarkAreas)
                    .SetRemoveHighlights(command.removeHighlights)
                    .SetRemoveDarkNoise(command.removeDarkNoise);

                if (!string.IsNullOrEmpty(command.inputFolderPath))
                {
                    var loadOp = service.LoadInputFolderAsync(command.inputFolderPath);
                    loadOp.Execute();
                    if (loadOp.error != null)
                    {
                        var width = 0;
                        var height = 0;
                        if (loadOp.data != null && loadOp.data.baseTexture != null)
                        {
                            width = loadOp.data.baseTexture.width;
                            height = loadOp.data.baseTexture.height;
                        }
                        var errors = new List<string>();
                        DelightingHelpers.GetErrorMessagesFrom(((ProcessException)loadOp.error).errorCode, errors, width, height);
                        for (int i = 0; i < errors.Count; i++)
                            UnityDebug.LogError(errors[i]);

                        return;
                    }

                    input
                        .SetBaseTexture(loadOp.data.baseTexture)
                        .SetNormalsTexture(loadOp.data.normalsTexture)
                        .SetBentNormalsTexture(loadOp.data.bentNormalsTexture)
                        .SetAmbientOcclusionTexture(loadOp.data.ambientOcclusionTexture)
                        .SetPositionTexture(loadOp.data.positionTexture)
                        .SetMaskTexture(loadOp.data.maskTexture);

                    service.SetInput(loadOp.data);
                }
                else
                {
                    input
                        .SetBaseTexture(command.baseTexturePath)
                        .SetNormalsTexture(command.normalsTexturePath)
                        .SetBentNormalsTexture(command.bentNormalsTexturePath)
                        .SetAmbientOcclusionTexture(command.ambientOcclusionTexturePath)
                        .SetPositionTexture(command.positionTexturePath)
                        .SetMaskTexture(command.maskTexturePath);
                    service.SetInput(input);
                }

                var processOp = service.ProcessAsync(new ProcessArgs { fromStep = ProcessStep.Gather, calculateResult = true });
                processOp.Execute();

                if (processOp.error != null)
                {
                    UnityDebug.LogException(processOp.error);
                    return;
                }

                var bytes = processOp.data.result.EncodeToPNG();
                File.WriteAllBytes(command.delightedTexturePath, bytes);
            }

            static Command Parse(string[] args)
            {
                var result = new Command();
                var index = 0;
                while (index < args.Length)
                {
                    switch (args[index])
                    {
                        case "-base":
                            ++index;
                            if (index < args.Length)
                                result.baseTexturePath = args[index];
                            break;
                        case "-normals":
                            ++index;
                            if (index < args.Length)
                                result.normalsTexturePath = args[index];
                            break;
                        case "-bentNormals":
                            ++index;
                            if (index < args.Length)
                                result.bentNormalsTexturePath = args[index];
                            break;
                        case "-ao":
                            ++index;
                            if (index < args.Length)
                                result.ambientOcclusionTexturePath = args[index];
                            break;
                        case "-position":
                            ++index;
                            if (index < args.Length)
                                result.positionTexturePath = args[index];
                            break;
                        case "-mask":
                            ++index;
                            if (index < args.Length)
                                result.maskTexturePath = args[index];
                            break;
                        case "-inputFolder":
                            ++index;
                            if (index < args.Length)
                                result.inputFolderPath = args[index];
                            break;
                        case "-output":
                            ++index;
                            if (index < args.Length)
                                result.delightedTexturePath = args[index];
                            break;
                        case "-separateDarkAreas":
                            {
                            ++index;
                            float value;
                            if (index < args.Length && float.TryParse(args[index], out value))
                                result.separateDarkAreas = value;
                            break;
                            }
                        case "-forceLocalDelighting":
                            {
                                ++index;
                                float value;
                                if (index < args.Length && float.TryParse(args[index], out value))
                                    result.forceLocalDelighting = value;
                                break;
                            }
                        case "-removeHighlights":
                            {
                                ++index;
                                float value;
                                if (index < args.Length && float.TryParse(args[index], out value))
                                    result.removeHighlights = value;
                                break;
                            }
                        case "-removeDarkNoise":
                            {
                                ++index;
                                float value;
                                if (index < args.Length && float.TryParse(args[index], out value))
                                    result.removeDarkNoise = value;
                                break;
                            }
                        case "-switchYZ":
                            {
                                ++index;
                                bool value;
                                if (index < args.Length && bool.TryParse(args[index], out value))
                                    result.switchYZ = value;
                                break;
                            }

                    }
                    ++index;
                }
                return result;
            }
        }
    }
}
