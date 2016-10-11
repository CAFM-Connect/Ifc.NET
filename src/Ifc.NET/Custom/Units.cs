using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifc4
{
    public class Units : List<Unit>
    {
        private static Units m_Units;

        public static Units Current
        {
            get
            {
                if (m_Units == null)
                    m_Units = new Units();
                return m_Units;
            }
        }

        private Units()
        {
            InitializeSupportedDocumentUnits();
        }

        public Unit AddNewUnit(string key, string displayText, bool supportedByIfc = true)
        {
            if(this.Exists(item => item.Key.Equals(key)))
                return this.FirstOrDefault(item => item.Key == key);

            Unit unit = new Unit(this, key, displayText, supportedByIfc);
            this.Add(unit);
            return unit;
        }

        public bool TryGetUnit(string key, out Unit unit)
        {
            unit = null;
            if (String.IsNullOrEmpty(key))
                return false;

            unit = this.FirstOrDefault(item => item.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
            return (unit != null);
        }

        private void InitializeSupportedDocumentUnits()
        {
            // --------------------------------------------
            AddNewUnit("m", "m - Meter");
            AddNewUnit("dm", "dm - Dezimeter");
            AddNewUnit("cm", "cm - Zentimeter");
            AddNewUnit("mm", "mm - Millimeter");
                        
            AddNewUnit("m²", "m² - Quadratmeter");
            AddNewUnit("dm²", "dm² - Quadratdezimeter");
            AddNewUnit("cm²", "cm² - Quadratzentimeter");
            AddNewUnit("mm²", "mm² - Quadratmillimeter");

            AddNewUnit("m³", "m³ - Kubikmeter");
            AddNewUnit("dm³", "dm³ - Kubikdezimeter");
            AddNewUnit("cm³", "cm³ - Kubikzentimeter");
            AddNewUnit("mm³", "mm³ - Kubikmillimeter");


            // 1m³ = 1000dm³
            AddNewUnit("l", "l - Liter");
            // --------------------------------------------
            AddNewUnit("V", "V - Volt");
            AddNewUnit("J", "J - Joule");
            AddNewUnit("°C", "°C - Grad Celsius");

            AddNewUnit("A", "A - Ampere");
            AddNewUnit("Ah", "Ah - Amperstunde");
            AddNewUnit("Ohm", "Ω - Ohm - Elektrischer Widerstand");

            AddNewUnit("Hz", "Hz - Frequenz");

            AddNewUnit("N", "N - Newton");

            AddNewUnit("N/m²", "N/m² - Druck");

            // --------------------------------------------
            AddNewUnit("kg", "kg - Kilogramm");
            AddNewUnit("g", "g - Gramm");

            // --------------------------------------------
            AddNewUnit("s", "s - Sekunde");
            AddNewUnit("min", "min - Minute");
            AddNewUnit("h", "h - Stunde");

            AddNewUnit("m/s", "m/s - Geschwindigkeit");
            AddNewUnit("m/s²", "m/s² - Beschleinigung");
            // --------------------------------------------
            AddNewUnit("m³/h", "m³/h - Volumenstrom");

            AddNewUnit("kg/m³", "kg/m³ - Dichte");

            AddNewUnit("W", "W - Leistung"); 

            AddNewUnit("kW", "kW - Kilowatt");

            AddNewUnit("Pa", "Pa - Druck"); // 1N/m²

            AddNewUnit("db", "db - Dezibel");

            AddNewUnit("W/m²", "W/m² - Wärmestromdichte");
        }

        private static IfcSIUnit AddIfcSIUnit(
                                        Ifc4.Document document,
                                        Unit unit,
                                        IfcUnitEnum? ifcUnitEnum,
                                        IfcSIPrefix? ifcSIPrefix,
                                        IfcSIUnitName? ifcSIUnitName
                                    )
        {

            if (document.Project.UnitsInContext == null)
                document.Project.UnitsInContext = new IfcUnitAssignment();
            var ifcUnitAssignment = document.Project.UnitsInContext;

            if (ifcUnitAssignment.Units == null)
                ifcUnitAssignment.Units = new IfcUnitAssignmentUnits();
            var ifcUnitAssignmentUnits = ifcUnitAssignment.Units;

            IfcSIUnit ifcSIUnit = new IfcSIUnit()
            {
                Id = document.GetNextSid(),
            };

            if (ifcUnitEnum.HasValue)
            {
                ifcSIUnit.UnitType = ifcUnitEnum.Value;
                ifcSIUnit.UnitTypeSpecified = true;
            }

            if (ifcSIPrefix.HasValue)
            {
                ifcSIUnit.Prefix = ifcSIPrefix.Value;
                ifcSIUnit.PrefixSpecified = true;
            }

            if (ifcSIUnitName.HasValue)
            {
                ifcSIUnit.Name = ifcSIUnitName.Value;
                ifcSIUnit.NameSpecified = true;
            }

            ifcUnitAssignmentUnits.Items.Add(ifcSIUnit);
            return ifcSIUnit;
        }

        private static IfcDerivedUnit AddIfcDerivedUnit(
                                        Ifc4.Document document,
                                        Unit unit,
                                        IfcDerivedUnit ifcDerivedUnit
                                    )
        {

            if (document.Project.UnitsInContext == null)
                document.Project.UnitsInContext = new IfcUnitAssignment();
            var ifcUnitAssignment = document.Project.UnitsInContext;

            if (ifcUnitAssignment.Units == null)
                ifcUnitAssignment.Units = new IfcUnitAssignmentUnits();
            var ifcUnitAssignmentUnits = ifcUnitAssignment.Units;

            ifcUnitAssignmentUnits.Items.Add(ifcDerivedUnit);
            return ifcDerivedUnit;
        }


        internal Entity AddUnitToIfcDocument(Ifc4.Document document, Ifc4.Unit unit)
        {
            if (document == null)
                throw new ArgumentNullException("AddUnitToIfcDocument(Ifc4.Document document, ...)");

            if (unit == null)
                throw new ArgumentNullException("AddUnitToIfcDocument(..., Unit unit)");

            Entity entity = null;

            if (unit.Key == "m")
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Lengthunit, null, IfcSIUnitName.Metre);
            }
            else if (unit.Key == "dm")
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Lengthunit, IfcSIPrefix.Deci, IfcSIUnitName.Metre);
            }
            else if (unit.Key == "cm")
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Lengthunit, IfcSIPrefix.Centi, IfcSIUnitName.Metre);
            }
            else if (unit.Key == "mm")
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Lengthunit, IfcSIPrefix.Milli, IfcSIUnitName.Metre);
            }
            else if (unit.Key == "m²")
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Areaunit, null, IfcSIUnitName.SquareMetre);
            }
            else if (unit.Key == "dm²")
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Areaunit, IfcSIPrefix.Deci, IfcSIUnitName.SquareMetre);
            }
            else if (unit.Key == "cm²")
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Areaunit, IfcSIPrefix.Centi, IfcSIUnitName.SquareMetre);
            }
            else if (unit.Key == "mm²")
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Areaunit, IfcSIPrefix.Milli, IfcSIUnitName.SquareMetre);
            }
            else if (unit.Key == "m³")
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Volumeunit, null, IfcSIUnitName.CubicMetre);
            }
            else if (unit.Key == "dm³")
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Volumeunit, IfcSIPrefix.Deci, IfcSIUnitName.CubicMetre);
            }
            else if (unit.Key == "cm³")
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Volumeunit, IfcSIPrefix.Centi, IfcSIUnitName.CubicMetre);
            }
            else if (unit.Key == "mm³")
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Volumeunit, IfcSIPrefix.Milli, IfcSIUnitName.CubicMetre);
            }
            // 1Liter = 1dm³
            // 1m³ = 1000dm³
            else if (unit.Key == "l" || unit.Key == "Liter")
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Volumeunit, IfcSIPrefix.Deci, IfcSIUnitName.CubicMetre);
            }
            else if (unit.Key == "V")
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Electricvoltageunit, null, IfcSIUnitName.Volt);
            }
            else if (unit.Key == "J")
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Energyunit, null, IfcSIUnitName.Joule);
            }
            else if (unit.Key == "°C")
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Thermodynamictemperatureunit, null, IfcSIUnitName.DegreeCelsius);
            }
            else if (unit.Key == "A")
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Electriccurrentunit, null, IfcSIUnitName.Ampere);
            }
            else if (unit.Key == "Hz") // 1/s
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Frequencyunit, null, IfcSIUnitName.Hertz);
            }
            else if (unit.Key == "N")
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Forceunit, null, IfcSIUnitName.Newton);
            }
            else if (unit.Key == "kg")
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Massunit, IfcSIPrefix.Kilo, IfcSIUnitName.Gram);
            }
            else if (unit.Key == "g")
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Massunit, null, IfcSIUnitName.Gram);
            }
            else if (unit.Key == "s")
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Timeunit, null, IfcSIUnitName.Second);
            }
            else if (unit.Key == "m/s")
            {
                IfcDerivedUnit ifcDerivedUnit = new IfcDerivedUnit()
                {
                    Id = document.GetNextSid(),
                    UnitType = IfcDerivedUnitEnum.Linearvelocityunit,
                    UnitTypeSpecified = true,
                    Elements = new IfcDerivedUnitElements()
                };

                ifcDerivedUnit.Elements.IfcDerivedUnitElement.AddRange(
                    new IfcDerivedUnitElement[]
                        {
                            new IfcDerivedUnitElement()
                            {
                                Id = document.GetNextSid(),
                                Exponent = 1,
                                ExponentSpecified = true,
                                Unit = new IfcSIUnit()
                                {
                                    UnitType = IfcUnitEnum.Lengthunit,
                                    UnitTypeSpecified = true,
                                }
                            },
                            new IfcDerivedUnitElement()
                            {
                                Id = document.GetNextSid(),
                                Exponent = -1,
                                ExponentSpecified = true,
                                Unit = new IfcSIUnit()
                                {
                                    UnitType = IfcUnitEnum.Timeunit,
                                    UnitTypeSpecified = true,
                                }
                            }
                        }
                    );

                entity = AddIfcDerivedUnit(document, unit, ifcDerivedUnit);
            }
            else if (unit.Key == "m/s²")
            {
                IfcDerivedUnit ifcDerivedUnit = new IfcDerivedUnit()
                {
                    Id = document.GetNextSid(),
                    UnitType = IfcDerivedUnitEnum.Accelerationunit,
                    UnitTypeSpecified = true,
                    Elements = new IfcDerivedUnitElements()
                };

                ifcDerivedUnit.Elements.IfcDerivedUnitElement.AddRange(
                    new IfcDerivedUnitElement[]
                        {
                            new IfcDerivedUnitElement()
                            {
                                Id = document.GetNextSid(),
                                Exponent = 1,
                                ExponentSpecified = true,
                                Unit = new IfcSIUnit()
                                {
                                    UnitType = IfcUnitEnum.Lengthunit,
                                    UnitTypeSpecified = true,
                                }
                            },
                            new IfcDerivedUnitElement()
                            {
                                Id = document.GetNextSid(),
                                Exponent = -2,
                                ExponentSpecified = true,
                                Unit = new IfcSIUnit
                                {
                                    UnitType = IfcUnitEnum.Timeunit,
                                    UnitTypeSpecified = true,
                                }
                            }
                        }
                    );

                entity = AddIfcDerivedUnit(document, unit, ifcDerivedUnit);
            }
            else if (unit.Key == "m³/h")
            {
                IfcDerivedUnit ifcDerivedUnit = new IfcDerivedUnit()
                {
                    Id = document.GetNextSid(),
                    UnitType = IfcDerivedUnitEnum.Volumetricflowrateunit,
                    UnitTypeSpecified = true,
                    Elements = new IfcDerivedUnitElements()
                };

                ifcDerivedUnit.Elements.IfcDerivedUnitElement.AddRange(
                    new IfcDerivedUnitElement[]
                        {
                            new IfcDerivedUnitElement()
                            {
                                Id = document.GetNextSid(),
                                Exponent = 3,
                                ExponentSpecified = true,
                                Unit = new IfcSIUnit()
                                {
                                    UnitType = IfcUnitEnum.Lengthunit,
                                    UnitTypeSpecified = true
                                }
                            },
                            new IfcDerivedUnitElement()
                            {
                                Id = document.GetNextSid(),
                                Exponent = -1,
                                ExponentSpecified = true,
                                Unit = new IfcConversionBasedUnit
                                {
                                    Name = "Hour",
                                    UnitType = IfcUnitEnum.Timeunit,
                                    UnitTypeSpecified = true,
                                    ConversionFactor = new IfcMeasureWithUnit()
                                    {
                                        ValueComponent = new IfcMeasureWithUnitValueComponent()
                                        {
                                            Item = new IfcTimeMeasurewrapper()
                                            {
                                                Value = 3600.0
                                            }
                                        },
                                        UnitComponent = new IfcMeasureWithUnitUnitComponent()
                                        {
                                            Item = new IfcSIUnit()
                                            {
                                                UnitType = IfcUnitEnum.Timeunit,
                                                UnitTypeSpecified = true
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    );
                entity = AddIfcDerivedUnit(document, unit, ifcDerivedUnit);
            }
            else if (unit.Key == "Ah")
            {
                IfcDerivedUnit ifcDerivedUnit = new IfcDerivedUnit()
                {
                    Id = document.GetNextSid(),
                    UnitType = IfcDerivedUnitEnum.Heatfluxdensityunit,
                    UnitTypeSpecified = true,
                    Elements = new IfcDerivedUnitElements()
                };

                ifcDerivedUnit.Elements.IfcDerivedUnitElement.AddRange(
                    new IfcDerivedUnitElement[]
                        {
                            new IfcDerivedUnitElement()
                            {
                                Id = document.GetNextSid(),
                                Exponent = 1,
                                ExponentSpecified = true,
                                Unit = new IfcSIUnit()
                                {
                                    UnitType = IfcUnitEnum.Electriccurrentunit,
                                    UnitTypeSpecified = true
                                }
                            },
                            new IfcDerivedUnitElement()
                            {
                                Id = document.GetNextSid(),
                                Exponent = -1,
                                ExponentSpecified = true,
                                Unit = new IfcConversionBasedUnit
                                {
                                    Name = "Hour",
                                    UnitType = IfcUnitEnum.Timeunit,
                                    UnitTypeSpecified = true,
                                    ConversionFactor = new IfcMeasureWithUnit()
                                    {
                                        ValueComponent = new IfcMeasureWithUnitValueComponent()
                                        {
                                            Item = new IfcTimeMeasurewrapper()
                                            {
                                                Value = 3600.0
                                            }
                                        },
                                        UnitComponent = new IfcMeasureWithUnitUnitComponent()
                                        {
                                            Item = new IfcSIUnit()
                                            {
                                                UnitType = IfcUnitEnum.Timeunit,
                                                UnitTypeSpecified = true
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    );
                entity = AddIfcDerivedUnit(document, unit, ifcDerivedUnit);
            }
            else if (unit.Key == "Ohm")
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Electricresistanceunit, null, IfcSIUnitName.Ohm);
            }
            else if (unit.Key == "kg/m³")
            {
                IfcDerivedUnit ifcDerivedUnit = new IfcDerivedUnit()
                {
                    Id = document.GetNextSid(),
                    UnitType = IfcDerivedUnitEnum.Massdensityunit,
                    UnitTypeSpecified = true,
                    Elements = new IfcDerivedUnitElements()
                };

                ifcDerivedUnit.Elements.IfcDerivedUnitElement.AddRange(
                    new IfcDerivedUnitElement[]
                        {
                            new IfcDerivedUnitElement()
                            {
                                Id = document.GetNextSid(),
                                Exponent = 1,
                                ExponentSpecified = true,
                                Unit = new IfcSIUnit()
                                {
                                    UnitType = IfcUnitEnum.Massunit,
                                    UnitTypeSpecified = true,
                                }
                            },
                            new IfcDerivedUnitElement()
                            {
                                Id = document.GetNextSid(),
                                Exponent = -3,
                                ExponentSpecified = true,
                                Unit = new IfcSIUnit
                                {
                                    UnitType = IfcUnitEnum.Lengthunit,
                                    UnitTypeSpecified = true,
                                }
                            }
                        }
                    );
                entity = AddIfcDerivedUnit(document, unit, ifcDerivedUnit);
            }
            else if (unit.Key == "N/m²")
            {
                IfcDerivedUnit ifcDerivedUnit = new IfcDerivedUnit()
                {
                    Id = document.GetNextSid(),
                    UnitType = IfcDerivedUnitEnum.Planarforceunit,
                    UnitTypeSpecified = true,
                    Elements = new IfcDerivedUnitElements()
                };

                ifcDerivedUnit.Elements.IfcDerivedUnitElement.AddRange(
                    new IfcDerivedUnitElement[]
                        {
                            new IfcDerivedUnitElement()
                            {
                                Id = document.GetNextSid(),
                                Exponent = 1,
                                ExponentSpecified = true,
                                Unit = new IfcSIUnit()
                                {
                                    UnitType = IfcUnitEnum.Forceunit,
                                    UnitTypeSpecified = true,
                                }
                            },
                            new IfcDerivedUnitElement()
                            {
                                Id = document.GetNextSid(),
                                Exponent = -2,
                                ExponentSpecified = true,
                                Unit = new IfcSIUnit()
                                {
                                    UnitType = IfcUnitEnum.Lengthunit,
                                    UnitTypeSpecified = true,
                                }
                            }
                        }
                    );
                entity = AddIfcDerivedUnit(document, unit, ifcDerivedUnit);
            }
            else if (unit.Key == "W/m²")
            {
                IfcDerivedUnit ifcDerivedUnit = new IfcDerivedUnit()
                {
                    Id = document.GetNextSid(),
                    UnitType = IfcDerivedUnitEnum.Heatfluxdensityunit,
                    UnitTypeSpecified = true,
                    Elements = new IfcDerivedUnitElements()
                };

                ifcDerivedUnit.Elements.IfcDerivedUnitElement.AddRange(
                    new IfcDerivedUnitElement[]
                        {
                            new IfcDerivedUnitElement()
                            {
                                Id = document.GetNextSid(),
                                Exponent = 1,
                                ExponentSpecified = true,
                                Unit = new IfcSIUnit()
                                {
                                    UnitType = IfcUnitEnum.Powerunit,
                                    UnitTypeSpecified = true,
                                }
                            },
                            new IfcDerivedUnitElement()
                            {
                                Id = document.GetNextSid(),
                                Exponent = -2,
                                ExponentSpecified = true,
                                Unit = new IfcSIUnit
                                {
                                    UnitType = IfcUnitEnum.Lengthunit,
                                    UnitTypeSpecified = true,
                                }
                            }
                        }
                    );
                entity = AddIfcDerivedUnit(document, unit, ifcDerivedUnit);
            }
            else if (unit.Key == "W")
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Powerunit, null, IfcSIUnitName.Watt);
            }
            else if (unit.Key == "kW")
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Powerunit, IfcSIPrefix.Kilo, IfcSIUnitName.Watt);
            }
            else if (unit.Key == "Pa")
            {
                entity = AddIfcSIUnit(document, unit, IfcUnitEnum.Pressureunit, null, IfcSIUnitName.Pascal);
            }
            else if (unit.Key == "db")
            {
                IfcDerivedUnit ifcDerivedUnit = new IfcDerivedUnit()
                {
                    Id = document.GetNextSid(),
                    UnitType = IfcDerivedUnitEnum.Soundpowerlevelunit,
                    UnitTypeSpecified = true,
                    Elements = new IfcDerivedUnitElements()
                };

                ifcDerivedUnit.Elements.IfcDerivedUnitElement.AddRange(
                    new IfcDerivedUnitElement[]
                        {
                            new IfcDerivedUnitElement()
                            {
                                Id = document.GetNextSid(),
                                Exponent = 1,
                                ExponentSpecified = true,
                                Unit = new IfcSIUnit()
                                {
                                    UnitType = IfcUnitEnum.Powerunit,
                                    UnitTypeSpecified = true,
                                }
                            },
                            new IfcDerivedUnitElement()
                            {
                                Id = document.GetNextSid(),
                                Exponent = -1,
                                ExponentSpecified = true,
                                Unit = new IfcSIUnit
                                {
                                    UnitType = IfcUnitEnum.Powerunit,
                                    UnitTypeSpecified = true,
                                }
                            }
                        }
                    );
                entity = AddIfcDerivedUnit(document, unit, ifcDerivedUnit);
            }

            if (entity == null)
                throw new NullReferenceException("AddUnitToIfcDocument -  Entity = null");

            return entity;

        }

    }
}
