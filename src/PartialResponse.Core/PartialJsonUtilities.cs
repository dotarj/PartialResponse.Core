// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PartialResponse.Core
{
    public static class PartialJsonUtilities
    {
        public static void RemovePropertiesAndArrayElements(object value, JsonWriter jsonWriter, JsonSerializer jsonSerializer, Func<string, bool> shouldSerialize)
        {
            if (value == null)
            {
                jsonSerializer.Serialize(jsonWriter, value);
            }
            else
            {
                var token = JToken.FromObject(value, jsonSerializer);

                var array = token as JArray;

                if (array != null)
                {
                    RemoveArrayElements(array, null, shouldSerialize, new Dictionary<string, bool>());

                    array.WriteTo(jsonWriter);
                }
                else
                {
                    var @object = token as JObject;

                    if (@object != null)
                    {
                        RemoveObjectProperties(@object, null, shouldSerialize, new Dictionary<string, bool>());

                        @object.WriteTo(jsonWriter);
                    }
                    else
                    {
                        token.WriteTo(jsonWriter);
                    }
                }
            }
        }

        private static void RemoveArrayElements(JArray array, string currentPath, Func<string, bool> shouldSerialize, Dictionary<string, bool> cache)
        {
            array.OfType<JObject>()
                .ToList()
                .ForEach(childObject => RemoveObjectProperties(childObject, currentPath, shouldSerialize, cache));

            RemoveArrayIfEmpty(array);
        }

        private static void RemoveArrayIfEmpty(JArray array)
        {
            if (array.Count == 0)
            {
                if (array.Parent is JProperty && array.Parent.Parent != null)
                {
                    array.Parent.Remove();
                }
                else if (array.Parent is JArray)
                {
                    array.Remove();
                }
            }
        }

        private static void RemoveObjectProperties(JObject @object, string currentPath, Func<string, bool> shouldSerialize, Dictionary<string, bool> cache)
        {
            @object.Properties()
                .Where(property =>
                {
                    var path = PathUtilities.CombinePath(currentPath, property.Name);

                    if (cache.ContainsKey(path))
                    {
                        return cache[path];
                    }

                    var result = !shouldSerialize(path);

                    cache.Add(path, result);

                    return result;
                })
                .ToList()
                .ForEach(property => property.Remove());

            @object.Properties()
                .Where(property => property.Value is JObject)
                .ToList()
                .ForEach(property =>
                {
                    var path = PathUtilities.CombinePath(currentPath, property.Name);

                    RemoveObjectProperties((JObject)property.Value, path, shouldSerialize, cache);
                });

            @object.Properties()
                .Where(property => property.Value is JArray)
                .ToList()
                .ForEach(property =>
                {
                    var path = PathUtilities.CombinePath(currentPath, property.Name);

                    RemoveArrayElements((JArray)property.Value, path, shouldSerialize, cache);
                });

            RemoveObjectIfEmpty(@object);
        }

        private static void RemoveObjectIfEmpty(JObject @object)
        {
            if (!@object.Properties().Any())
            {
                if (@object.Parent is JProperty && @object.Parent.Parent != null)
                {
                    @object.Parent.Remove();
                }
                else if (@object.Parent is JArray)
                {
                    @object.Remove();
                }
            }
        }
    }
}
