using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace RestApiDemo.WebApi.PatchRequests
{
    /// <summary>
    /// Class is used in most HttpPatch requests for updating the DB entity
    /// with properties that are present in BODY of request.
    /// Properties that are not passed preserve their previous values.
    /// </summary>
    public static class PartialUpdateHelper
    {
        /// <summary>
        /// Used in most HttpPatch requests for updating the DB entity
        /// with properties that are present in BODY of request.
        /// Properties that are not passed preserve their previous values.
        /// </summary>
        public static void Update<TObjectToUpdate, TObjectWithNewValues>(
            this TObjectToUpdate objectToUpdate,
            TObjectWithNewValues objectWithNewValues)
            where TObjectWithNewValues : PatchRequest<TObjectToUpdate>
        {
            List<PropertyInfo> objectWithNewValuesProperties = typeof(TObjectWithNewValues)
                .GetProperties()
                .Where(x => x.GetGetMethod() != null)
                .ToList();
            Type typeToUpdate = typeof(TObjectToUpdate);

            foreach (var propertyInNewValuesObject in objectWithNewValuesProperties)
            {
                string propertyName = propertyInNewValuesObject.Name;
                if (objectWithNewValues.IsFieldPresent(propertyName))
                {
                    var propertyInObjectToUpdate = typeToUpdate.GetProperty(propertyName);
                    if (propertyInObjectToUpdate != null)
                    {
                        var notMappedAttribute =
                            propertyInNewValuesObject.GetCustomAttribute(
                                typeof(NotMappedAttribute));
                        if (notMappedAttribute == null)
                        {
                            object propertyValue =
                                propertyInNewValuesObject.GetValue(objectWithNewValues);
                            Type propertyType =
                                Nullable.GetUnderlyingType(propertyInObjectToUpdate.PropertyType) ??
                                propertyInObjectToUpdate.PropertyType;
                            if (propertyType.IsEnum)
                            {
                                if (!Enum.IsDefined(propertyType, Convert.ToInt32(propertyValue)))
                                {
                                    throw new ValidationException(
                                        $"Type mismatch for {propertyName}, value: {propertyValue}");
                                }
                            }

                            if (propertyValue is IEnumerable && !(propertyValue is string))
                            {
                                continue;
                            }


                            propertyInObjectToUpdate.SetValue(objectToUpdate, propertyValue);
                        }
                    }
                }
            }
        }
    }
}