﻿//---------------------------------------------------------------------
// <copyright file="PrimitiveValuesRoundtripJsonLightTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Tests.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.Spatial;
using Microsoft.Test.OData.DependencyInjection;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.Roundtrip.JsonLight
{
    public class PrimitiveValuesRoundtripJsonLightTests
    {
        private EdmModel model;
        private IServiceProvider container;

        public PrimitiveValuesRoundtripJsonLightTests()
        {
            model = new EdmModel();
        }

        [Fact]
        public void BinaryRoundtripJsonLightTest()
        {
            var values = new byte[][]
            {
                new byte[0],
                new byte[] { 0 },
                new byte[] { 42, Byte.MinValue, Byte.MaxValue },
            };

            this.VerifyPrimitiveValuesRoundtripWithTypeInformation(values, "Edm.Binary");
            this.VerifyPrimitiveValuesDoNotRoundtripWithoutTypeInformation(values);
        }

        [Fact]
        public void BinaryPayloadAsStringRoundtripJsonLightTest()
        {
            var values = new byte[][]
            {
                new byte[0],
                new byte[] { 0 },
                new byte[] { 42, Byte.MinValue, Byte.MaxValue },
            };

            var expectedValues = new string[]
            {
                Convert.ToBase64String(values[0]),
                Convert.ToBase64String(values[1]),
                Convert.ToBase64String(values[2])
            };

            this.container = ContainerBuilderHelper.BuildContainer(
                builder => builder.AddService<ODataPayloadValueConverter, BinaryFieldAsStringPrimitivePayloadValueConverter>(ServiceLifetime.Singleton));

            this.VerifyPrimitiveValuesRoundtripWithTypeInformationAndWithExpectedValues(values, "Edm.Binary", expectedValues);
            this.VerifyPrimitiveValuesRoundtripWithTypeInformation(expectedValues, "Edm.Binary");
        }

        [Fact]
        public void BooleanRoundtripJsonLightTest()
        {
            var values = new bool[]
            {
                true, 
                false,
            };

            this.VerifyPrimitiveValuesRoundtripWithTypeInformation(values, "Edm.Boolean");
            this.VerifyPrimitiveValuesRoundtripWithoutTypeInformation(values);
        }

        [Fact]
        public void ByteRoundtripJsonLightTest()
        {
            var values = new byte[]
            {
                0,
                42,
                Byte.MaxValue,
                Byte.MinValue,
            };

            this.VerifyPrimitiveValuesRoundtripWithTypeInformation(values, "Edm.Byte");
            this.VerifyPrimitiveValuesDoNotRoundtripWithoutTypeInformation(values);
        }

        [Fact]
        public void DateRoundtripJsonLightTest()
        {
            var values = new Date[]
            {
                new Date(2012, 4, 13),
                new Date(1, 1, 1),
                new Date(9999, 12, 31),
                new Date(), 
            };

            this.VerifyPrimitiveValuesRoundtripWithTypeInformation(values, "Edm.Date");
            this.VerifyPrimitiveValuesDoNotRoundtripWithoutTypeInformation(values);
        }

        [Fact]
        public void DateTimeOffsetRoundtripJsonLightTest()
        {
            var values = new DateTimeOffset[]
            {
                new DateTimeOffset(2012, 4, 13, 2, 43, 10, TimeSpan.Zero),
                new DateTimeOffset(2012, 4, 13, 2, 43, 10, 215, TimeSpan.FromMinutes(840)),
                new DateTimeOffset(2012, 4, 13, 2, 43, 10, 215, TimeSpan.FromMinutes(-840)),
                new DateTimeOffset(2012, 4, 13, 2, 43, 10, 215, TimeSpan.FromMinutes(123)),
                new DateTimeOffset(2012, 4, 13, 2, 43, 10, 215, TimeSpan.FromMinutes(-42)),
                DateTimeOffset.MinValue,
                DateTimeOffset.MaxValue,
            };

            this.VerifyPrimitiveValuesRoundtripWithTypeInformation(values, "Edm.DateTimeOffset");
            this.VerifyPrimitiveValuesDoNotRoundtripWithoutTypeInformation(values);
        }

        [Fact]
        public void DecimalRoundtripJsonLightTest()
        {
            var values = new decimal[]
            {
                0,
                1,
                -1,
                Decimal.MinValue,
                Decimal.MaxValue,
                10^-28,
                10^28,
            };

            this.VerifyPrimitiveValuesRoundtripWithTypeInformation(values, "Edm.Decimal");
            this.VerifyPrimitiveValuesDoNotRoundtripWithoutTypeInformation(values);
        }

        [Fact]
        public void DecimalRoundTripJsonLightTestWithIeee754CompatibleFalse()
        {
            var values = new decimal[]
            {
                0,
                1,
                -1,
                Decimal.MinValue,
                Decimal.MaxValue,
                10^-28,
                10^28,
            };

            this.VerifyPrimitiveValuesRoundtripWithTypeInformationIeee754CompatibleFalse(values, "Edm.Decimal");

            // precision lose for Ieee754Compatible=false
            this.VerifyPrimitiveValuesDoNotRoundtripWithoutTypeInformationIeee754CompatibleFalse(new[] { Decimal.MaxValue, Decimal.MinValue });
        }

        [Fact]
        public void DoubleRoundtripJsonLightTest()
        {
            IEnumerable<double> valuesWrittenAsDigits = new double[]
            {
                0,
                42,
                42.42,
                Double.MaxValue,
                Double.MinValue,
                -4.42330604244772E-305,
                42E20,
            };

            IEnumerable<double> valuesWrittenAsString = new double[]
            {
                Double.PositiveInfinity,
                Double.NegativeInfinity,
                Double.NaN,                       
            };

            this.VerifyPrimitiveValuesRoundtripWithTypeInformation(valuesWrittenAsDigits.Concat(valuesWrittenAsString), "Edm.Double");
            this.VerifyPrimitiveValuesRoundtripWithoutTypeInformation(valuesWrittenAsDigits);
            this.VerifyPrimitiveValuesDoNotRoundtripWithoutTypeInformation(valuesWrittenAsString);
        }

        [Fact]
        public void GuidRoundtripJsonLightTest()
        {
            var values = new Guid[]
            {
                new Guid(0, 0, 0, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }),
                new Guid("C8864E5E-BDB1-4FB2-A1C4-8F8E49C271EA"),
                Guid.Empty,
            };

            this.VerifyPrimitiveValuesRoundtripWithTypeInformation(values, "Edm.Guid");
            this.VerifyPrimitiveValuesDoNotRoundtripWithoutTypeInformation(values);
        }

        [Fact]
        public void Int16RoundtripJsonLightTest()
        {
            var values = new Int16[]
            {
                0,
                42,
                -43,
                Int16.MaxValue,
                Int16.MinValue,
            };

            this.VerifyPrimitiveValuesRoundtripWithTypeInformation(values, "Edm.Int16");
            this.VerifyPrimitiveValuesDoNotRoundtripWithoutTypeInformation(values);
        }

        [Fact]
        public void Int32RoundtripJsonLightTest()
        {
            var values = new Int32[]
            {
                0,
                42,
                -43,
                Int32.MaxValue,
                Int32.MinValue,
            };

            this.VerifyPrimitiveValuesRoundtripWithTypeInformation(values, "Edm.Int32");
            this.VerifyPrimitiveValuesRoundtripWithoutTypeInformation(values);
        }

        [Fact]
        public void Int64RoundtripJsonLightTest()
        {
            var values = new Int64[]
            {
                0,
                42,
                -43,
                Int64.MaxValue,
                Int64.MinValue,
            };

            this.VerifyPrimitiveValuesRoundtripWithTypeInformation(values, "Edm.Int64");
            this.VerifyPrimitiveValuesDoNotRoundtripWithoutTypeInformation(values);
        }

        [Fact]
        public void Int64RoundTripJsonLightTestWithIeee754CompatibleFalse()
        {
            var values = new Int64[]
            {
                0,
                42,
                -43,
                Int64.MaxValue,
                Int64.MinValue,
            };

            this.VerifyPrimitiveValuesRoundtripWithTypeInformationIeee754CompatibleFalse(values, "Edm.Int64");

            // precision lose for Ieee754Compatible=false
            this.VerifyPrimitiveValuesDoNotRoundtripWithoutTypeInformationIeee754CompatibleFalse(new[] { Int64.MaxValue, Int64.MinValue });
        }

        [Fact]
        public void SByteRoundtripJsonLightTest()
        {
            var values = new SByte[]
            {
                0,
                42,
                -43,
                SByte.MaxValue,
                SByte.MinValue,
            };

            this.VerifyPrimitiveValuesRoundtripWithTypeInformation(values, "Edm.SByte");
            this.VerifyPrimitiveValuesDoNotRoundtripWithoutTypeInformation(values);
        }

        [Fact]
        public void StringRoundtripJsonLightTest()
        {
            var values = new string[]
            {
                string.Empty,
                " ",
                "testvalue",
                "TestValue",
                "\r\n\t",
                "\"",
                "\'",
            };

            this.VerifyPrimitiveValuesRoundtripWithTypeInformation(values, "Edm.String");
            this.VerifyPrimitiveValuesRoundtripWithoutTypeInformation(values);
        }

        [Fact]
        public void SingleRoundtripJsonLightTest()
        {
            var values = new Single[]
            {
                0,
                42,
                (float)-43.43,
                Single.MaxValue,
                Single.MinValue,
                Single.PositiveInfinity,
                Single.NegativeInfinity,
                Single.NaN,
            };

            this.VerifyPrimitiveValuesRoundtripWithTypeInformation(values, "Edm.Single");
            this.VerifyPrimitiveValuesDoNotRoundtripWithoutTypeInformation(values);
        }

        [Fact]
        public void TimeRoundtripJsonLightTest()
        {
            var values = new TimeSpan[]
            {
                new TimeSpan(1, 2, 3, 4, 5),
                TimeSpan.Zero,
                TimeSpan.MinValue,
                TimeSpan.MaxValue,
            };

            this.VerifyPrimitiveValuesRoundtripWithTypeInformation(values, "Edm.Duration");
            this.VerifyPrimitiveValuesDoNotRoundtripWithoutTypeInformation(values);
        }

        [Fact]
        public void TimeOfDayRoundtripJsonLightTest()
        {
            var values = new TimeOfDay[]
            {
                new TimeOfDay(10, 5, 30, 90),
                new TimeOfDay(TimeOfDay.MinTickValue),
                new TimeOfDay(TimeOfDay.MaxTickValue),
                new TimeOfDay(), 
            };

            this.VerifyPrimitiveValuesRoundtripWithTypeInformation(values, "Edm.TimeOfDay");
            this.VerifyPrimitiveValuesDoNotRoundtripWithoutTypeInformation(values);
        }

        [Fact]
        public void GeographyMultiLineStringRoundtripJsonLightTest()
        {
            var values = new GeographyMultiLineString[]
            {
                GeographyFactory.MultiLineString().LineString(0, 0).LineTo(0, 0).Build(), 
                GeographyFactory.MultiLineString().LineString(-90.0, -90.0).LineTo(0, 0).LineString(90.0, 90.0).LineTo(0, 0).Build(), 
                GeographyFactory.MultiLineString().LineString(-90.0, 0).LineTo(0, 0).LineString(0, 0).LineTo(0, 0).LineString(0, 90.0).LineTo(0, 0).Build()
            };
            this.VerifyPrimitiveValuesRoundtripWithTypeInformation(values, "GeographyMultiLineString");
        }

        [Fact]
        public void GeometryCollectionRoundtripJsonLightTest()
        {
            var values = new GeometryCollection[]
            {
                GeometryFactory.Collection().Build(),
                GeometryFactory.Collection().Point(0, 0).Build()
            };
            this.VerifyPrimitiveValuesRoundtripWithTypeInformation(values, "GeometryCollection");
        }

        [Fact]
        public void UInt16RoundtripJsonLightTest()
        {
            var uint16 = new EdmTypeDefinition("NS", "UInt16", EdmPrimitiveTypeKind.Double);
            var uint16Ref = new EdmTypeDefinitionReference(uint16, true);
            this.model.AddElement(uint16);
            this.model.SetPrimitiveValueConverter(uint16Ref, UInt16ValueConverter.Instance);
            var values = new[]
            {
                (UInt16)123,
                UInt16.MinValue,
                UInt16.MaxValue
            };
            this.VerifyUIntValuesRoundtripWithTypeInformation(values, "NS.UInt16");
        }

        [Fact]
        public void UInt32RoundtripJsonLightTest()
        {
            var uint32 = new EdmTypeDefinition("NS", "UInt32", EdmPrimitiveTypeKind.String);
            var uint32Ref = new EdmTypeDefinitionReference(uint32, true);
            this.model.AddElement(uint32);
            this.model.SetPrimitiveValueConverter(uint32Ref, UInt32ValueConverter.Instance);
            var values = new[]
            {
                (UInt32)456,
                UInt32.MinValue,
                UInt32.MaxValue
            };
            this.VerifyUIntValuesRoundtripWithTypeInformation(values, "NS.UInt32");
        }

        [Fact]
        public void UInt64RoundtripJsonLightTest()
        {
            var uint64 = new EdmTypeDefinition("NS", "UInt64", EdmPrimitiveTypeKind.String);
            var uint64Ref = new EdmTypeDefinitionReference(uint64, true);
            this.model.AddElement(uint64);
            this.model.SetPrimitiveValueConverter(uint64Ref, UInt64ValueConverter.Instance);
            var values = new[]
            {
                (UInt64)456,
                UInt64.MinValue,
                UInt64.MaxValue
            };
            this.VerifyUIntValuesRoundtripWithTypeInformation(values, "NS.UInt64");
        }

        [Fact]
        public void UnsignedIntAndTypeDefinitionRoundtripJsonLightIntegrationTest()
        {
            var model = new EdmModel();

            var uint16 = new EdmTypeDefinition("MyNS", "UInt16", EdmPrimitiveTypeKind.Double);
            var uint16Ref = new EdmTypeDefinitionReference(uint16, false);
            model.AddElement(uint16);
            model.SetPrimitiveValueConverter(uint16Ref, UInt16ValueConverter.Instance);

            var uint64 = new EdmTypeDefinition("MyNS", "UInt64", EdmPrimitiveTypeKind.String);
            var uint64Ref = new EdmTypeDefinitionReference(uint64, false);
            model.AddElement(uint64);
            model.SetPrimitiveValueConverter(uint64Ref, UInt64ValueConverter.Instance);

            var guidType = new EdmTypeDefinition("MyNS", "Guid", EdmPrimitiveTypeKind.Int64);
            var guidRef = new EdmTypeDefinitionReference(guidType, true);
            model.AddElement(guidType);

            var personType = new EdmEntityType("MyNS", "Person");
            personType.AddKeys(personType.AddStructuralProperty("ID", uint64Ref));
            personType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            personType.AddStructuralProperty("FavoriteNumber", uint16Ref);
            personType.AddStructuralProperty("Age", model.GetUInt32("MyNS", true));
            personType.AddStructuralProperty("Guid", guidRef);
            personType.AddStructuralProperty("Weight", EdmPrimitiveTypeKind.Double);
            personType.AddStructuralProperty("Money", EdmPrimitiveTypeKind.Decimal);
            model.AddElement(personType);

            var container = new EdmEntityContainer("MyNS", "Container");
            var peopleSet = container.AddEntitySet("People", personType);
            model.AddElement(container);

            var stream = new MemoryStream();
            IODataResponseMessage message = new InMemoryMessage { Stream = stream };
            message.StatusCode = 200;

            var writerSettings = new ODataMessageWriterSettings();
            writerSettings.SetServiceDocumentUri(new Uri("http://host/service"));

            var messageWriter = new ODataMessageWriter(message, writerSettings, model);
            var entryWriter = messageWriter.CreateODataResourceWriter(peopleSet);

            var entry = new ODataResource
            {
                TypeName = "MyNS.Person",
                Properties = new[]
                {
                    new ODataProperty
                    {
                        Name = "ID",
                        Value = UInt64.MaxValue
                    },
                    new ODataProperty
                    {
                        Name = "Name",
                        Value = "Foo"
                    },
                    new ODataProperty
                    {
                        Name = "FavoriteNumber",
                        Value = (UInt16)250
                    },
                    new ODataProperty
                    {
                        Name = "Age",
                        Value = (UInt32)123
                    },
                    new ODataProperty
                    {
                        Name = "Guid",
                        Value = Int64.MinValue
                    },
                    new ODataProperty
                    {
                        Name = "Weight",
                        Value = 123.45
                    },
                    new ODataProperty
                    {
                        Name = "Money",
                        Value = Decimal.MaxValue
                    }
                }
            };

            entryWriter.WriteStart(entry);
            entryWriter.WriteEnd();
            entryWriter.Flush();

            stream.Position = 0;

            StreamReader reader = new StreamReader(stream);
            string payload = reader.ReadToEnd();
            Assert.Equal(payload, "{\"@odata.context\":\"http://host/service/$metadata#People/$entity\",\"ID\":\"18446744073709551615\",\"Name\":\"Foo\",\"FavoriteNumber\":250.0,\"Age\":123,\"Guid\":-9223372036854775808,\"Weight\":123.45,\"Money\":79228162514264337593543950335}");

#if NETCOREAPP1_0
            stream = new MemoryStream(Encoding.GetEncoding(0).GetBytes(payload));
#else
            stream = new MemoryStream(Encoding.Default.GetBytes(payload));
#endif
            message = new InMemoryMessage { Stream = stream };
            message.StatusCode = 200;

            var readerSettings = new ODataMessageReaderSettings();

            var messageReader = new ODataMessageReader(message, readerSettings, model);
            var entryReader = messageReader.CreateODataResourceReader(peopleSet, personType);
            Assert.True(entryReader.Read());
            var entryReaded = entryReader.Item as ODataResource;

            var propertiesReaded = entryReaded.Properties.ToList();
            var propertiesGiven = entry.Properties.ToList();
            Assert.Equal(propertiesReaded.Count, propertiesGiven.Count);
            for (int i = 0; i < propertiesReaded.Count; ++i)
            {
                Assert.Equal(propertiesReaded[i].Name, propertiesGiven[i].Name);
                Assert.Equal(propertiesReaded[i].Value.GetType(), propertiesGiven[i].Value.GetType());
                Assert.Equal(propertiesReaded[i].Value, propertiesGiven[i].Value);
            }
        }

        private void VerifyUIntValuesRoundtripWithTypeInformation(IEnumerable clrValues, string edmTypeDefinitionName)
        {
            var typeReference = new EdmTypeDefinitionReference((IEdmTypeDefinition)this.model.FindType(edmTypeDefinitionName), true);
            foreach (ODataVersion version in new ODataVersion[] { ODataVersion.V4, ODataVersion.V401 })
            {
                foreach (object clrValue in clrValues)
                {
                    this.VerifyPrimitiveValueRoundtrips(clrValue, typeReference, version, isIeee754Compatible: true);
                }
            }
        }

        private void VerifyPrimitiveValuesRoundtripWithTypeInformation(IEnumerable clrValues, string edmTypeName)
        {
            var typeReference = new EdmPrimitiveTypeReference((IEdmPrimitiveType)this.model.FindType(edmTypeName), true);
            foreach (object clrValue in clrValues)
            {
                foreach (ODataVersion version in new ODataVersion[] { ODataVersion.V4, ODataVersion.V401 })
                {
                    this.VerifyPrimitiveValueRoundtrips(clrValue, typeReference, version,isIeee754Compatible: true);
                }
            }
        }


        private void VerifyPrimitiveValuesRoundtripWithTypeInformationAndWithExpectedValues(Array clrValues, string edmTypeName, Array expectedValues)
        {
            var typeReference = new EdmPrimitiveTypeReference((IEdmPrimitiveType)this.model.FindType(edmTypeName), true);

            Assert.Equal(clrValues.Length, expectedValues.Length);

            for (int iterator = 0; iterator < clrValues.Length; iterator++)
            {
                object clrValue = clrValues.GetValue(iterator);
                object expectedValue = expectedValues.GetValue(iterator);
                foreach (ODataVersion version in new ODataVersion[] { ODataVersion.V4, ODataVersion.V401 })
                {
                    this.VerifyPrimitiveValueRoundtrips(clrValue, typeReference, version, isIeee754Compatible: true, expectedValue: expectedValue);
                }
            }
        }

        private void VerifyPrimitiveValuesRoundtripWithoutTypeInformation(IEnumerable clrValues)
        {
            foreach (object clrValue in clrValues)
            {
                foreach (ODataVersion version in new ODataVersion[] { ODataVersion.V4, ODataVersion.V401 })
                {
                    this.VerifyPrimitiveValueRoundtrips(clrValue, null, version, isIeee754Compatible: true);
                }
            }
        }

        private void VerifyPrimitiveValuesDoNotRoundtripWithoutTypeInformation(IEnumerable clrValues)
        {
            foreach (object clrValue in clrValues)
            {
                foreach (ODataVersion version in new ODataVersion[] { ODataVersion.V4, ODataVersion.V401 })
                {
                    this.VerifyPrimitiveValueDoesNotRoundtrip(clrValue, null, version, isIeee754Compatible: true);
                }
            }
        }

        private void VerifyPrimitiveValuesDoNotRoundtripWithoutTypeInformationIeee754CompatibleFalse(IEnumerable clrValues)
        {
            foreach (object clrValue in clrValues)
            {
                foreach (ODataVersion version in new ODataVersion[] { ODataVersion.V4, ODataVersion.V401 })
                {
                    this.VerifyPrimitiveValueDoesNotRoundtrip(clrValue, null, version, isIeee754Compatible: false);
                }
            }
        }

        private void VerifyPrimitiveValuesRoundtripWithTypeInformationIeee754CompatibleFalse(IEnumerable clrValues, string edmTypeName)
        {
            var typeReference = new EdmPrimitiveTypeReference((IEdmPrimitiveType)this.model.FindType(edmTypeName), true);
            foreach (object clrValue in clrValues)
            {
                foreach (ODataVersion version in new ODataVersion[] { ODataVersion.V4, ODataVersion.V401 })
                {
                    this.VerifyPrimitiveValueRoundtrips(clrValue, typeReference, version, isIeee754Compatible: false);
                }
            }
        }

        private void VerifyPrimitiveValueRoundtrips(object clrValue, IEdmTypeReference typeReference, ODataVersion version, bool isIeee754Compatible)
        {
            VerifyPrimitiveValueRoundtrips(clrValue, typeReference, version, isIeee754Compatible, clrValue);
        }

        private void VerifyPrimitiveValueRoundtrips(object clrValue, IEdmTypeReference typeReference, ODataVersion version, bool isIeee754Compatible, object expectedValue)
        {
            var actualValue = this.WriteThenReadValue(clrValue, typeReference, version, isIeee754Compatible);

            if (expectedValue is byte[])
            {
                Assert.Equal((byte[])actualValue, (byte[])expectedValue);
            }
            else
            {
                Assert.Equal(actualValue.GetType(), expectedValue.GetType());
                Assert.Equal(actualValue, expectedValue);
            }
        }

        private void VerifyPrimitiveValueDoesNotRoundtrip(object clrValue, IEdmTypeReference typeReference, ODataVersion version, bool isIeee754Compatible)
        {
            var actualValue = this.WriteThenReadValue(clrValue, typeReference, version, isIeee754Compatible);

            if (clrValue != null && clrValue is byte[])
            {
                if (actualValue != null && actualValue is byte[])
                {
                    Assert.NotEqual((byte[])actualValue, (byte[])clrValue);
                }
                else if (actualValue != null)
                {
                    Assert.NotEqual(actualValue.GetType(), clrValue.GetType());
                }

                Assert.True(true);
            }
            else
            {
                Assert.NotEqual(actualValue, clrValue);
            }
        }

        private object WriteThenReadValue(object clrValue, IEdmTypeReference typeReference, ODataVersion version, bool isIeee754Compatible)
        {
            var stream = new MemoryStream();

            var settings = new ODataMessageWriterSettings { Version = version };
            settings.SetServiceDocumentUri(new Uri("http://odata.org/test/"));

            var mediaType = isIeee754Compatible
                ? new ODataMediaType("application", "json", new KeyValuePair<string, string>("IEEE754Compatible", "true"))
                : new ODataMediaType("application", "json");

            var messageInfoForWriter = new ODataMessageInfo
            {
                MessageStream = new NonDisposingStream(stream),
                MediaType = mediaType,
                Encoding = Encoding.UTF8,
                IsResponse = true,
                IsAsync = false,
                Model = this.model,
                Container = this.container
            };

            using (var outputContext = new ODataJsonLightOutputContext(messageInfoForWriter, settings))
            {
                var serializer = new ODataJsonLightValueSerializer(outputContext);
                serializer.WritePrimitiveValue(clrValue, typeReference);
            }

            stream.Position = 0;

            var messageInfoForReader = new ODataMessageInfo
            {
                Encoding = Encoding.UTF8,
                IsResponse = true,
                MediaType = mediaType,
                IsAsync = false,
                Model = this.model,
                MessageStream = stream,
                Container = this.container
            };

            object actualValue;
            using (var inputContext = new ODataJsonLightInputContext(
                messageInfoForReader, new ODataMessageReaderSettings()))
            {
                var deserializer = new ODataJsonLightPropertyAndValueDeserializer(inputContext);
                deserializer.JsonReader.Read();
                actualValue = deserializer.ReadNonEntityValue(
                    /*payloadTypeName*/ null,
                    typeReference,
                    /*propertyAndAnnotationCollector*/ null,
                    /*collectionValidator*/ null,
                    /*validateNullValue*/ true,
                    /*isTopLevel*/ true,
                    /*insideResourceValue*/ false,
                    /*propertyName*/ null);

            }

            return actualValue;
        }
    }

    internal class UInt16ValueConverter : IPrimitiveValueConverter
    {
        private static readonly IPrimitiveValueConverter instance = new UInt16ValueConverter();

        internal static IPrimitiveValueConverter Instance
        {
            get { return instance; }
        }

        public object ConvertToUnderlyingType(object value)
        {
            return Convert.ToDouble(value);
        }

        public object ConvertFromUnderlyingType(object value)
        {
            return Convert.ToUInt16(value);
        }
    }

    internal class UInt32ValueConverter : IPrimitiveValueConverter
    {
        private static readonly IPrimitiveValueConverter instance = new UInt32ValueConverter();

        internal static IPrimitiveValueConverter Instance
        {
            get { return instance; }
        }

        public object ConvertToUnderlyingType(object value)
        {
            return Convert.ToString(value);
        }

        public object ConvertFromUnderlyingType(object value)
        {
            return Convert.ToUInt32(value);
        }
    }

    internal class UInt64ValueConverter : IPrimitiveValueConverter
    {
        private static readonly IPrimitiveValueConverter instance = new UInt64ValueConverter();

        internal static IPrimitiveValueConverter Instance
        {
            get { return instance; }
        }

        public object ConvertToUnderlyingType(object value)
        {
            return Convert.ToString(value);
        }

        public object ConvertFromUnderlyingType(object value)
        {
            return Convert.ToUInt64(value);
        }
    }
}
