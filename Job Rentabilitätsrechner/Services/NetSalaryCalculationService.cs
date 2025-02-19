﻿using Job_Rentabilitätsrechner.Interfaces;
using System.Formats.Asn1;

namespace Job_Rentabilitätsrechner.Pages
{
    public class NetSalaryCalculationService : INetSalaryCalculationService
    {
        public float CalculateNetSalary(float grossSalary, int taxClass, bool churchTax, float kirchensteuerRate, bool isSachsen)
        {
            float lohnsteuer = CalculateLohnsteuer(grossSalary, taxClass);
            float solidaritätszuschlag = lohnsteuer * 0.055f;

            float kirchensteuerbetrag = churchTax ? lohnsteuer * kirchensteuerRate : 0f;

            float rentenversicherung = grossSalary * 0.093f; //9,3 % für Rentenversicherung

            float krankenversicherung = grossSalary * 0.073f; // 7,3% für Krankenversicherung

            // pflegerversicherung abhängig ob sachsen oder nicht: 1,525% nicht sachsen. 2.205% sachsen
            float pflegeversicherung = isSachsen ? grossSalary * 0.02025f : grossSalary * 0.01525f;

            float arbeitslosenversicherung = grossSalary * 0.013f; // 1,3% für Arbeitslosenversicherung

            float netto = grossSalary - (lohnsteuer + kirchensteuerbetrag +
                                          rentenversicherung + krankenversicherung +
                                          pflegeversicherung + arbeitslosenversicherung);

            return netto;
        }

        public float CalculateNewNetSalary(float newGrossSalary, int taxClass, bool churchTax, float churchTaxRate, bool isSachsen)
        {
            return CalculateNetSalary(newGrossSalary, taxClass, churchTax, churchTaxRate, isSachsen);
        }

        public float CalculateChurchTaxRate(bool churchTax, string state)
        {
            if (!churchTax)
                return 0;

            if (state == "bayern" || state == "bawue")
                return 0.08f;

            return 0.09f;
        }

        public float AdjustFuelConsumption(float consumption, string transmissionType, int? gearCount)
        {
            if (transmissionType == "automatic")
            {
                consumption *= 1.10f; // Erhöhung um 10% bei Automatikgetriebe

                if (gearCount.HasValue && gearCount > 5)
                {
                    consumption *= 0.95f; // Reduzierung um 5% bei mehr als 5 Gängen
                }
            }

            return consumption;
        }

        public void CalculateNetSalaries(float grossSalary, float newGrossSalary, int taxClass, bool churchTax, float churchTaxRate,
            bool useExternalNetto, bool isSachsen, float? externalOldNetSalary, float? externalNewNetSalary, out float netSalary, out float newNetSalary)
        {
            // Berechnung für altes Netto basierend auf externer Eingabe
            if (useExternalNetto)
            {
                netSalary = CalculateNetSalaryWithExternalInput(externalOldNetSalary, taxClass);
                newNetSalary = CalculateNetSalaryWithExternalInput(externalNewNetSalary, taxClass);
            }
            else
            {
                netSalary = CalculateNetSalary(grossSalary, taxClass, churchTax, churchTaxRate, isSachsen);
                newNetSalary = CalculateNetSalary(newGrossSalary, taxClass, churchTax, churchTaxRate, isSachsen);
            }
        }
        public float AdjustNetSalaryForSachsen(float externalNetSalary, bool isSachsen)
        {
            if(isSachsen)
            {
                float sachsenPflegeversicherung = externalNetSalary * 0.02025f;
                return externalNetSalary - sachsenPflegeversicherung;
            }
            return externalNetSalary;
        }

            private float CalculateNetSalaryWithExternalInput(float? externalNetSalary, int taxClass)
        {
            if (externalNetSalary.HasValue && externalNetSalary > 0)
            {
                float netSalary = externalNetSalary.Value;
                float soli = CalculateSoliForExternalNetto(netSalary, taxClass);
                return netSalary - soli;
            }

            // Fallback-Wert oder Exception, falls kein externer Netto-Wert vorhanden
            return 0;
        }

        public bool ShouldApplySoli(float grossSalary, int taxClass)
        {
            float soliThreshold = (taxClass == 3 || taxClass == 4) ? 136826f : 68413f;
            return grossSalary > soliThreshold;
        }

        public float CalculateLohnsteuer(float brutto, int steuerklasse)
        {
            float steuer = 0f;
            float grundfreibetrag = 10908f;
            float entlastungsbetragAlleinerziehende = 0f;

            var steuerStufen = new (float Grenze, float Steuersatz)[]
            {
            (17005, 0.14f), // Beginn der Steuerprogression nach Grundfreibetrag
            (66760, 0.24f),
            (277825, 0.42f),
            (float.MaxValue, 0.45f)
            };

            switch (steuerklasse)
            {
                case 1:
                    grundfreibetrag = 10908f;
                    break;
                case 2:
                    grundfreibetrag = 10908f;
                    entlastungsbetragAlleinerziehende = 4260f;
                    break;
                case 3:
                    grundfreibetrag = 20000f;
                    break;
                case 4:
                    grundfreibetrag = 10908f;
                    break;
                case 5:
                    grundfreibetrag = 0f;
                    break;
                case 6:
                    grundfreibetrag = 0f;
                    break;
                default:
                    throw new ArgumentException("Ungültige Steuerklasse");
            }

            float zuVersteuerndesEinkommen = brutto - grundfreibetrag - entlastungsbetragAlleinerziehende;

            if (zuVersteuerndesEinkommen <= 0)
            {
                steuer = 0;
            }
            else
            {
                float vorherigeGrenze = 0f;
                foreach (var stufe in steuerStufen)
                {
                    if (zuVersteuerndesEinkommen > stufe.Grenze)
                    {
                        steuer += (stufe.Grenze - vorherigeGrenze) * stufe.Steuersatz;
                        vorherigeGrenze = stufe.Grenze;
                    }
                    else
                    {
                        steuer += (zuVersteuerndesEinkommen - vorherigeGrenze) * stufe.Steuersatz;
                        break;
                    }
                }
            }

            return steuer;
        }

        public float CalculateGrossAfterDeductions(float brutto, int steuerklasse, bool kirchensteuer, float kirchensteuerRate = 0.09f)
        {
            float lohnsteuer = CalculateLohnsteuer(brutto, steuerklasse);
            float kirchensteuerbetrag = kirchensteuer ? lohnsteuer * kirchensteuerRate : 0f;
            return brutto - (lohnsteuer + kirchensteuerbetrag);
        }

        public float CalculateSoliForExternalNetto(float externalNetSalary, int taxClass)
        {
            float soliThreshold = (taxClass == 3 || taxClass == 4) ? 136826f : 68413f;
            float lohnsteuer = externalNetSalary * 0.18f;
            float solidaritätszuschlag = lohnsteuer * 0.055f;
            return externalNetSalary > soliThreshold ? solidaritätszuschlag : 0f;
        }

        public bool CheckSoliThreshold(float grossSalary, int taxClass)
        {
            float soliThreshold = (taxClass == 3 || taxClass == 4) ? 136826f : 68413f;
            return grossSalary > soliThreshold;
        }
    }

}