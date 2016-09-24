using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifc4.Pset
{
    public class Reader
    {

        //private bool ImportPset(string psetFullName, CeElements elements)
        //{
        //    try
        //    {
        //        //CeWorkspace.CurrentWorkspace.RaiseBeforeCommandExceuted(this, "ImportFromPset");
        //        var propertySetDef = CatalogueEditor.Core.XmlProcessing<CatalogueEditor.Core.PSet.PropertySetDef>.Read(psetFullName);

        //        int result = 0;

        //        foreach (var applicableClasse in propertySetDef.ApplicableClasses)
        //        {
        //            string ifcClassName = applicableClasse.Trim();
        //            string ifcPrefix = "Ifc";
        //            string className = ifcClassName;
        //            if (className.StartsWith(ifcPrefix, StringComparison.InvariantCultureIgnoreCase))
        //                className = className.Substring(ifcPrefix.Length);

        //            string number = className;
        //            string name = className;
        //            CeElement element = elements.AddNewElement(number, name);
        //            element.IfcEntity = ifcClassName;

        //            string fileName = System.IO.Path.GetFileName(psetFullName);
        //            DateTime dt = DateTime.Now;

        //            element.Comment = String.Format("Import aus PSet ({0}) am {1} um {2} Uhr", fileName, dt.ToShortDateString(), dt.ToShortTimeString());

        //            foreach (var propertyDef in propertySetDef.PropertyDefs)
        //            {
        //                string propertyDefinition = String.Empty;
        //                string propertyDefinitionAlias = String.Empty;
        //                string propertyName = String.Empty;
        //                string propertyNameAlias = String.Empty;
        //                object propertyValueDef;

        //                CeElementAttribute elementAttribute = element.ElementAttributes.AddNewElementAttribute();

        //                for (int i = 0; i < propertyDef.ItemsElementName.Length; i++)
        //                {
        //                    CatalogueEditor.Core.PSet.ItemsChoiceType elementName = propertyDef.ItemsElementName[i];
        //                    switch (elementName)
        //                    {
        //                        case CatalogueEditor.Core.PSet.ItemsChoiceType.Definition:
        //                            propertyDefinition = propertyDef.Items[i].ToString();
        //                            break;
        //                        case CatalogueEditor.Core.PSet.ItemsChoiceType.DefinitionAliases:


        //                            CatalogueEditor.Core.PSet.PropertyDefDefinitionAliases propertyDefDefinitionAliases = propertyDef.Items[i] as CatalogueEditor.Core.PSet.PropertyDefDefinitionAliases;
        //                            if (propertyDefDefinitionAliases != null && propertyDefDefinitionAliases.DefinitionAlias != null)
        //                            {
        //                                foreach (CatalogueEditor.Core.PSet.PropertyDefDefinitionAliasesDefinitionAlias propertyDefDefinitionAliasesDefinitionAlias in propertyDefDefinitionAliases.DefinitionAlias)
        //                                {
        //                                    if (propertyDefDefinitionAliasesDefinitionAlias.lang.Equals("de-DE", StringComparison.InvariantCultureIgnoreCase))
        //                                    {
        //                                        propertyDefinitionAlias = propertyDefDefinitionAliasesDefinitionAlias.Value;
        //                                        break;
        //                                    }
        //                                }
        //                            }
        //                            break;
        //                        case CatalogueEditor.Core.PSet.ItemsChoiceType.Name:
        //                            propertyName = propertyDef.Items[i].ToString();
        //                            break;
        //                        case CatalogueEditor.Core.PSet.ItemsChoiceType.NameAliases:

        //                            CatalogueEditor.Core.PSet.PropertyDefNameAliases propertyDefNameAliases = propertyDef.Items[i] as CatalogueEditor.Core.PSet.PropertyDefNameAliases;
        //                            if (propertyDefNameAliases != null && propertyDefNameAliases.NameAlias != null)
        //                            {
        //                                foreach (CatalogueEditor.Core.PSet.PropertyDefNameAliasesNameAlias propertyDefNameAliasesNameAlias in propertyDefNameAliases.NameAlias)
        //                                {
        //                                    if (propertyDefNameAliasesNameAlias.lang.Equals("de-DE", StringComparison.InvariantCultureIgnoreCase))
        //                                    {
        //                                        propertyNameAlias = propertyDefNameAliasesNameAlias.Value;
        //                                        break;
        //                                    }
        //                                }
        //                            }
        //                            break;
        //                        case CatalogueEditor.Core.PSet.ItemsChoiceType.PropertyType:
        //                            CatalogueEditor.Core.PSet.PropertyType propertyType = propertyDef.Items[i] as CatalogueEditor.Core.PSet.PropertyType;
        //                            SetPropertyTypeFromPset(elementAttribute, propertyType);
        //                            break;
        //                        case CatalogueEditor.Core.PSet.ItemsChoiceType.ValueDef:
        //                            propertyValueDef = propertyDef.Items[i];
        //                            break;
        //                        default:
        //                            break;
        //                    }
        //                }



        //                if (!String.IsNullOrEmpty(propertyNameAlias))
        //                {
        //                    elementAttribute.Name = propertyNameAlias;
        //                }
        //                else
        //                {
        //                    elementAttribute.Name = propertyName;
        //                }

        //                if (!String.IsNullOrEmpty(propertyDefinitionAlias))
        //                {
        //                    elementAttribute.Description = propertyDefinitionAlias;
        //                }
        //                else
        //                {
        //                    elementAttribute.Description = propertyDefinition;
        //                }

        //                string psetName = System.IO.Path.GetFileNameWithoutExtension(psetFullName);
        //                elementAttribute.IfcProperty = String.Format("{0}/{1}", psetName, propertyName);

        //                if (elementAttribute.Type == CeElementAttribute.AttributeType.Object)
        //                {
        //                    CeWorkspace.CurrentWorkspace.RaiseMessageLogged(this, new Exception(String.Format("Error '{0}'.", elementAttribute.Name)));
        //                    result |= 2;
        //                }
        //                else
        //                {
        //                    result |= 1;
        //                }
        //            }

        //        }

        //        return ((result & 2) != 2);
        //    }
        //    catch (Exception exc)
        //    {
        //        CeWorkspace.CurrentWorkspace.RaiseMessageLogged(this, exc);
        //        return false;
        //    }
        //    finally
        //    {
        //        // CeWorkspace.CurrentWorkspace.RaiseAfterCommandExceuted(this, "ImportFromPset");
        //    }
        //}


        //private void SetPropertyTypeFromPset(CeElementAttribute elementAttribute, CatalogueEditor.Core.PSet.PropertyType propertyType)
        //{

        //    if (propertyType == null || propertyType.Item == null)
        //        return;

        //    object obj;

        //    CatalogueEditor.Core.Ce.CeElementAttribute.AttributeType attributeType = CeElementAttribute.AttributeType.Object;
        //    CatalogueEditor.Core.PSet.DataType dataType = null;
        //    CatalogueEditor.Core.PSet.UnitType unitType = null;

        //    if (propertyType.Item is CatalogueEditor.Core.PSet.PropertyTypeTypeComplexProperty)
        //    {
        //        obj = (propertyType.Item as CatalogueEditor.Core.PSet.PropertyTypeTypeComplexProperty);
        //        CeWorkspace.CurrentWorkspace.RaiseMessageLogged(this, new Exception(String.Format("Error setting property type '{0}'.", propertyType.Item)));
        //    }
        //    else if (propertyType.Item is CatalogueEditor.Core.PSet.PropertyTypeTypePropertyBoundedValue)
        //    {
        //        dataType = (propertyType.Item as CatalogueEditor.Core.PSet.PropertyTypeTypePropertyBoundedValue).DataType;
        //        unitType = (propertyType.Item as CatalogueEditor.Core.PSet.PropertyTypeTypePropertyBoundedValue).UnitType;
        //    }
        //    else if (propertyType.Item is CatalogueEditor.Core.PSet.PropertyTypeTypePropertyEnumeratedValue)
        //    {
        //        CatalogueEditor.Core.PSet.PropertyTypeTypePropertyEnumeratedValueEnumList
        //            propertyTypeTypePropertyEnumeratedValueEnumList = (propertyType.Item as CatalogueEditor.Core.PSet.PropertyTypeTypePropertyEnumeratedValue).EnumList;
        //        foreach (var enumItem in propertyTypeTypePropertyEnumeratedValueEnumList.EnumItem)
        //        {
        //            elementAttribute.Items.AddNewItem(enumItem);
        //        }
        //        attributeType = CeElementAttribute.AttributeType.List;
        //    }

        //    else if (propertyType.Item is CatalogueEditor.Core.PSet.PropertyTypeTypePropertyListValue)
        //    {

        //        CatalogueEditor.Core.PSet.PropertyTypeTypePropertyListValueListValue propertyTypeTypePropertyListValueListValue = (propertyType.Item as CatalogueEditor.Core.PSet.PropertyTypeTypePropertyListValue).ListValue;

        //        dataType = propertyTypeTypePropertyListValueListValue.DataType;
        //        unitType = propertyTypeTypePropertyListValueListValue.UnitType;

        //        CeWorkspace.CurrentWorkspace.RaiseMessageLogged(this, new Exception(String.Format("Error setting property type '{0}'.", propertyType.Item)));
        //        //foreach (var a in propertyTypeTypePropertyListValueListValue.Values)
        //        //{
        //        //}

        //    }

        //    else if (propertyType.Item is CatalogueEditor.Core.PSet.PropertyTypeTypePropertyReferenceValue)
        //    {
        //        //dataType = (propertyType.Item as CatalogueEditor.Core.PSet.PropertyTypeTypePropertyReferenceValue).DataType;
        //        CeWorkspace.CurrentWorkspace.RaiseMessageLogged(this, new Exception(String.Format("Error setting property type '{0}'.", propertyType.Item)));
        //    }

        //    else if (propertyType.Item is CatalogueEditor.Core.PSet.PropertyTypeTypePropertySingleValue)
        //    {
        //        dataType = (propertyType.Item as CatalogueEditor.Core.PSet.PropertyTypeTypePropertySingleValue).DataType;
        //        unitType = (propertyType.Item as CatalogueEditor.Core.PSet.PropertyTypeTypePropertySingleValue).UnitType;
        //    }
        //    else if (propertyType.Item is CatalogueEditor.Core.PSet.PropertyTypeTypePropertyTableValue)
        //    {
        //        obj = (propertyType.Item as CatalogueEditor.Core.PSet.PropertyTypeTypePropertyTableValue);
        //        CeWorkspace.CurrentWorkspace.RaiseMessageLogged(this, new Exception(String.Format("Error setting property type '{0}'.", propertyType.Item)));
        //    }

        //    if (dataType != null)
        //    {
        //        switch (dataType.type)
        //        {
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcVolumeMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcTimeMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcThermodynamicTemperatureMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcSolidAngleMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcPositiveRatioMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcRatioMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcPositivePlaneAngleMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcPlaneAngleMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcParameterValue:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcNumericMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcMassMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcPositiveLengthMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcLengthMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcElectricCurrentMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcDescriptiveMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcCountMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcContextDependentMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcAreaMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcAmountOfSubstanceMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcLuminousIntensityMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcNormalisedRatioMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcComplexNumber:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcNonNegativeLengthMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcInteger:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcReal:
        //                attributeType = Ce.CeElementAttribute.AttributeType.Number;
        //                break;

        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcBoolean:
        //                attributeType = Ce.CeElementAttribute.AttributeType.Boolean;
        //                break;

        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcIdentifier:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcText:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcLabel:
        //                attributeType = Ce.CeElementAttribute.AttributeType.Text;
        //                break;

        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcLogical:
        //                attributeType = Ce.CeElementAttribute.AttributeType.Boolean;
        //                break;

        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcDateTime:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcDate:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcTime:
        //                attributeType = Ce.CeElementAttribute.AttributeType.Date;
        //                break;

        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcDuration:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcVolumetricFlowRateMeasure:
        //                attributeType = Ce.CeElementAttribute.AttributeType.Number;
        //                break;

        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcTimeStamp:
        //                attributeType = Ce.CeElementAttribute.AttributeType.Date;
        //                break;

        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcThermalTransmittanceMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcThermalResistanceMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcThermalAdmittanceMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcPressureMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcPowerMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcMassFlowRateMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcMassDensityMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcLinearVelocityMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcKinematicViscosityMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcIntegerCountRateMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcHeatFluxDensityMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcFrequencyMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcEnergyMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcElectricVoltageMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcDynamicViscosityMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcCompoundPlaneAngleMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcAngularVelocityMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcThermalConductivityMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcMolecularWeightMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcVaporPermeabilityMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcMoistureDiffusivityMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcIsothermalMoistureCapacityMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcSpecificHeatCapacityMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcMonetaryMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcMagneticFluxDensityMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcMagneticFluxMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcLuminousFluxMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcForceMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcInductanceMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcIlluminanceMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcElectricResistanceMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcElectricConductanceMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcElectricChargeMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcDoseEquivalentMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcElectricCapacitanceMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcAbsorbedDoseMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcRadioActivityMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcRotationalFrequencyMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcTorqueMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcAccelerationMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcLinearForceMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcLinearStiffnessMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcModulusOfSubgradeReactionMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcModulusOfElasticityMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcMomentOfInertiaMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcPlanarForceMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcRotationalStiffnessMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcShearModulusMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcLinearMomentMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcLuminousIntensityDistributionMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcCurvatureMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcMassPerLengthMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcModulusOfLinearSubgradeReactionMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcModulusOfRotationalSubgradeReactionMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcRotationalMassMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcSectionalAreaIntegralMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcSectionModulusMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcTemperatureGradientMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcThermalExpansionCoefficientMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcWarpingConstantMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcWarpingMomentMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcSoundPowerMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcSoundPressureMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcHeatingValueMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcPHMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcIonConcentrationMeasure:
        //            case CatalogueEditor.Core.PSet.DataTypeType.IfcTemperatureRateOfChangeMeasure:
        //                attributeType = Ce.CeElementAttribute.AttributeType.Number;
        //                break;
        //            default:
        //                break;
        //        }

        //    }

        //    elementAttribute.Type = attributeType;
        //    elementAttribute.Unit = unitType != null ? unitType.type.ToString() : String.Empty;

        //}

    }
}
