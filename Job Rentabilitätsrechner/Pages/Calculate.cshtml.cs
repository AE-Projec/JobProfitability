using Job_Rentabilitätsrechner.Controller;
using Job_Rentabilitätsrechner.Helpers;
using Job_Rentabilitätsrechner.Interfaces;
using Job_Rentabilitätsrechner.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;


namespace Job_Rentabilitätsrechner.Pages
{
    public class CalculateModel : PageModel
    {
        private readonly IWearAndTearCalculator _wearAndTearCalculator;
        private readonly ICommuteCostCalculationService _commutCostCalculationService;
        private readonly INetSalaryCalculationService _netSalaryCalculationService;
        private readonly IFuelConsumptionAdjustment _fuelConsumptionAdjustment;
        private readonly IGeocodingService _geocodingService;
        private readonly IDistanceService _distanceService;


        #region BindProperties
        [BindProperty]
        public float CommuteDistance { get; set; }
        [BindProperty]
        public float CommuteDistanceManual { get; set; }
        [BindProperty]
        public string? FuelPrice { get; set; }
        [BindProperty]
        public float DailyCost { get; set; }
        [BindProperty]
        public float FuelConsumption { get; set; } // Verbrauch in Litern pro 100 km
        [BindProperty]
        public string? FuelType { get; set; }
        [BindProperty]
        public float GrossSalary { get; set; }
        [BindProperty]
        public float NewGrossSalary { get; set; }
        [BindProperty]
        public bool IncludeSoli { get; set; }
        [BindProperty]
        public int TaxClass { get; set; }
        [BindProperty]
        public bool ChurchTax { get; set; }
        [BindProperty]
        public float NetSalary { get; set; }
        [BindProperty]
        public float NewNetSalary { get; set; }
        [BindProperty]
        public int WearLevel { get; set; } = 2; // Standardwert für Fahrzeugabnutzung
        [BindProperty]
        public bool IncludeWearAndTear { get; set; }
        [BindProperty]
        public string? Bundesland { get; set; }
        [BindProperty]
        public string? TransmissionType { get; set; }
        [BindProperty]
        public int? GearCount { get; set; }
        [BindProperty]
        public float ExternNetSalary { get; set; }
        [BindProperty]
        public int AverageCommuteDays { get; set; }
        [BindProperty]
        public int OldAverageCommuteDays { get; set; }
        [BindProperty]
        public float CommuteCostYear { get; set; }
        [BindProperty]
        public float OldCommuteCostYear { get; set; }
        [BindProperty]
        public bool UseExternalNetto { get; set; }
        [BindProperty]
        public bool UseCalculateDistance { get; set; }
        [BindProperty]
        public bool UseOldCalculateDistance { get; set; }
        [BindProperty]
        public string? FromLocation { get; set; }
        [BindProperty]
        public string? ToLocation { get; set; }
        [BindProperty]
        public int? CommuteDurationMinutesAutomatic { get; set; }
        [BindProperty]
        public int? CommuteDurationSecondsAutomatic { get; set; }
        [BindProperty]
        public float CommuteFullDistance { get; set; }
        [BindProperty]
        public float GrossAfterDeduction { get; set; }
        [BindProperty]
        public float OldGrossAfterDeduction { get; set; }
        [BindProperty]
        public bool IsSachsen { get; set; }
        [BindProperty]
        public bool IsMobileWork { get; set; }
        [BindProperty]
        public bool OldIsMobileWork { get; set; }
        [BindProperty]
        public bool IsDistanceDoubled { get; set; }
        [BindProperty]
        public float ExternOldNetSalary { get; set; }
        [BindProperty]
        public float OldCommuteDurationMinutesAutomatic { get; set; }
        [BindProperty]
        public float OldCommuteDurationSecondsAutomatic { get; set; }
        [BindProperty]
        public float OldCommuteDistance { get; set; }
        [BindProperty]
        public float OldCommuteDistanceManual { get; set; }
        #endregion

        #region Public Variables
        public float TotalCost { get; set; }
        public float OldTotalCost { get; set; }
        public float AdjustedSalary { get; set; }
        public float OldAdjustedSalary { get; set; }
        public float CommuteCost { get; set; }
        public float OldCommuteCost { get; set; }
        //public float SalaryDifference { get; set; }
        public bool IsCalculated { get; set; }
        public float MonthlyWearAndTear { get; set; }
        public float OldMonthlyWearAndTear { get; set; }
        public float TotalCostWearAndTear { get; set; }
        public float OldTotalCostWearAndTear { get; set; }
        public float MonthlyTotalCostWearAndTear { get; set; }
        public float OldMonthlyTotalCostWearAndTear { get; set; }
        public float AdjustedNetSalary { get; set; }
        public float OldAdjustedNetSalary { get; set; }
        public float AdjustedNetYearSalary { get; set; }
        public float OldAdjustedNetYearSalary { get; set; }
        public float AdjustedNetYearSalaryIntern { get; set; }
        public float OldAdjustedNetYearSalaryIntern { get; set; }
        public string FormattedCommuteDuration { get; set; }
        public string OldFormattedCommuteDuration { get; set; }
        public float MonthlyAdjustedNetYearSalary { get; set; }
        public float OldMonthlyAdjustedNetYearSalary { get; set; }
        public float MonthlyAdjustedNetYearSalaryInternal { get; set; }
        public float OldMonthlyAdjustedNetYearSalaryInternal { get; set; }
        public float MonthlyGrossAfterDeduction { get; set; }
        public float OldMonthlyGrossAfterDeduction { get; set; }
        public float FullDistance { get; set; }
        public float OldFullDistance { get; set; }
        public float CommuteDurationManual { get; set; }
        public float OldCommuteDurationManual { get; set; }
        public float WearAndTearYear { get; set; }
        public float OldWearAndTearYear { get; set; }

        #endregion

        public JsonResult OnGetCalculateNetSalary(float grossSalary, float newGrossSalary, int taxClass, bool churchTax, bool isSachsen)
        {
            try
            {
                float kirchensteuerRate = GetChurchTaxRate(churchTax, Bundesland);
                float netSalary = _netSalaryCalculationService.CalculateNetSalary(grossSalary, taxClass, churchTax, kirchensteuerRate, isSachsen);
                float newNetSalary = _netSalaryCalculationService.CalculateNetSalary(newGrossSalary, taxClass, churchTax, kirchensteuerRate, isSachsen);
                return new JsonResult(new { NetSalary = netSalary, NewNetSalary = newNetSalary });
            }
            catch (Exception ex)
            {

                return new JsonResult(new { Error = ex.Message });
            }
        }
        public CalculateModel(IWearAndTearCalculator wearAndTearCalculator,
            ICommuteCostCalculationService commutCostCalculationService,
            INetSalaryCalculationService netSalaryCalculationService,
            IFuelConsumptionAdjustment fuelConsumptionAdjustment,
            IGeocodingService geocodingService,
            IDistanceService distanceService)
        {
            _wearAndTearCalculator = wearAndTearCalculator;
            _commutCostCalculationService = commutCostCalculationService;
            _netSalaryCalculationService = netSalaryCalculationService;
            _fuelConsumptionAdjustment = fuelConsumptionAdjustment;
            _geocodingService = geocodingService;
            _distanceService = distanceService;

        }
        public void OnPost()
        {
            // Beide Jobs sind nicht mobil
            if (!OldIsMobileWork && !IsMobileWork)
            {
                if (!UseCalculateDistance && !UseOldCalculateDistance)
                {
                    CalculateForBothNonMobile();
                }
                else
                {
                    if (UseCalculateDistance && !UseOldCalculateDistance)
                    {
                        ApplyAutomaticDistanceValues();
                        var result = CommuteCalculationHelper.CalculateForNewMobileOldNonMobile(OldCommuteDistanceManual);
                        OldCommuteDistance = result.OldCommuteDistance;
                        OldFormattedCommuteDuration = result.OldFormattedDuration;

                    }
                    else if (!UseOldCalculateDistance && UseCalculateDistance)
                    {
                        ApplyOldAutomaticDistanceValues();

                        var result = CommuteCalculationHelper.CalculateForOldMobileNewNonMobile(CommuteDistanceManual);
                        CommuteDistance = result.CommuteDistance;
                        FormattedCommuteDuration = result.FormattedDuration;
                    }
                    //beide jobs automatisch berechnen (distanz)
                    else
                    {
                        ApplyAutomaticDistanceValues();
                        ApplyOldAutomaticDistanceValues();
                    }
                }
            }

            //alter job nicht mobil neuer job mobil
            else if (!OldIsMobileWork && IsMobileWork)
            {
                CalculateForOldNonMobileNewMobile();

                if (UseOldCalculateDistance)
                {
                    ApplyOldAutomaticDistanceValues();
                }
                else
                {
                    // Manuelle Berechnung für den alten Job, wenn nicht automatisch
                    var result = CommuteCalculationHelper.CalculateForNewMobileOldNonMobile(OldCommuteDistanceManual);
                    OldCommuteDistance = result.OldCommuteDistance;
                    OldFormattedCommuteDuration = result.OldFormattedDuration;
                }
            }

            //alter job ist mobil neuer ist nicht mobil
            else if (OldIsMobileWork && !IsMobileWork)
            {
                CalculateForOldMobileNewNonMobile();

                if (UseCalculateDistance)
                {
                    ApplyAutomaticDistanceValues();
                }
                else
                {
                    // Manuelle Berechnung für den neuen Job, wenn nicht automatisch
                    var result = CommuteCalculationHelper.CalculateForOldMobileNewNonMobile(CommuteDistanceManual);
                    CommuteDistance = result.CommuteDistance;
                    FormattedCommuteDuration = result.FormattedDuration;
                }
            }
            else if (OldIsMobileWork && IsMobileWork)
            {
                SetNoCommuteForBoth();
            }


            /*if (UseExternalNetto)

            {

                
            }*/



            // Berechnung der Kirchensteuer
            float kirchensteuerRate = GetChurchTaxRate(ChurchTax, Bundesland);

            // Anpassung des Kraftstoffverbrauchs
            float adjustedFuelConsumption = _fuelConsumptionAdjustment.AdjustFuelConsumption(FuelConsumption, TransmissionType, GearCount);


            // Berechnung des Bruttoeinkommens nach Abzügen neues Brutto
            float grossAfterDeductions = _netSalaryCalculationService.CalculateGrossAfterDeductions(NewGrossSalary, TaxClass, ChurchTax, kirchensteuerRate);

            // altes brutto 
            float oldGrossAfterDeductions = _netSalaryCalculationService.CalculateGrossAfterDeductions(GrossSalary, TaxClass, ChurchTax, kirchensteuerRate);


            // Berechnung der Nettogehälter
            // Lokale Variablen für out-Parameter
            float netSalary, newNetSalary;


            // Berechnung durchführen
            _netSalaryCalculationService.CalculateNetSalaries(GrossSalary, NewGrossSalary, TaxClass, ChurchTax, kirchensteuerRate, UseExternalNetto, IsSachsen, ExternOldNetSalary, ExternNetSalary, out netSalary, out newNetSalary);

            // Zuordnung der berechneten Werte zu den Properties
            NetSalary = netSalary;
            NewNetSalary = newNetSalary;

            // Überprüfung des Solidaritätszuschlags
            IncludeSoli = _netSalaryCalculationService.CheckSoliThreshold(GrossSalary, TaxClass);


            // Überprüfung der Pendeltage
            if (AverageCommuteDays < 0 || AverageCommuteDays > 7 && OldAverageCommuteDays < 0 || OldAverageCommuteDays > 7)
            {
                ModelState.AddModelError("AverageCommuteDays", "Die durchschnittliche Anzahl der Pendeltage pro Woche muss zwischen 0 und 7 liegen.");
                ModelState.AddModelError("OldAverageCommuteDays", "Die durchschnittliche Anzahl der Pendeltage pro Woche muss zwischen 0 und 7 liegen.");
                return;
            }


            // Lokale Variablen für Pendelkosten
            float commuteCost, commuteCostYear, totalCost, monthlyWearAndTear,
                totalCostWearAndTear, wearAndTearYear, adjustedNetSalary,
                adjustedSalary, totalAnnualCost;

            //lokale variablen für Alte Pendelkosten
            float oldCommuteCost, oldCommuteCostYear,
                oldTotalCost, oldTotalAnnualCost, oldAdjustedNetSalary,
                oldAdjustedSalary, oldTotalCostWearAndTear, oldMonthlyWearAndTear,
                oldWearAndTearYear;


            // Berechnung der Pendelkosten und Gesamtkosten

            if (float.TryParse(FuelPrice, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsedFuelPrice))
            {

                //float newDistanceForCalculation = UseCalculateDistance ? FullDistance : CommuteDistanceManual;
                float newDistanceForCalculation;
                var newResult = CommuteCalculationHelper.CalculateForOldMobileNewNonMobile(CommuteDistanceManual);
                if (UseCalculateDistance)
                {
                    newDistanceForCalculation = FullDistance;
                }
                else
                {
                    newDistanceForCalculation = CommuteDistanceManual;
                    FormattedCommuteDuration = newResult.FormattedDuration;
                }


                //neuer job
                _commutCostCalculationService.CalculateCommuteAndTotalCosts(parsedFuelPrice, adjustedFuelConsumption, newDistanceForCalculation,
                AverageCommuteDays, WearLevel, IncludeWearAndTear, NewGrossSalary, TaxClass, ChurchTax, IsSachsen, kirchensteuerRate,
                out commuteCost, out commuteCostYear, out totalCost, out monthlyWearAndTear, out totalCostWearAndTear,
                out wearAndTearYear, out adjustedNetSalary, out adjustedSalary, out totalAnnualCost);

                float oldDistanceForCalculation;
                var oldResult = CommuteCalculationHelper.CalculateForOldMobileNewNonMobile(OldCommuteDistanceManual);
                if (UseOldCalculateDistance)
                {
                    oldDistanceForCalculation = OldFullDistance;
                }
                else
                {
                    oldDistanceForCalculation = OldCommuteDistanceManual;
                    OldFormattedCommuteDuration = oldResult.FormattedDuration;
                }
                //alter job
                _commutCostCalculationService.OldCalculateCommuteAndTotalCosts(parsedFuelPrice, adjustedFuelConsumption, oldDistanceForCalculation,
                    OldAverageCommuteDays, WearLevel, IncludeWearAndTear, GrossSalary, TaxClass, ChurchTax, IsSachsen, kirchensteuerRate,
                    out oldCommuteCost, out oldCommuteCostYear, out oldTotalCost, out oldMonthlyWearAndTear, out oldTotalCostWearAndTear,
                    out oldWearAndTearYear, out oldAdjustedNetSalary, out oldAdjustedSalary, out oldTotalAnnualCost);

       

                //Alter Job
                OldAdjustedNetYearSalaryIntern = Math.Max(0, NetSalary - oldTotalAnnualCost);
                OldMonthlyAdjustedNetYearSalaryInternal = OldAdjustedNetYearSalaryIntern / 12;

                //Neuer Job
                AdjustedNetYearSalaryIntern = Math.Max(0, NewNetSalary - totalAnnualCost);
                MonthlyAdjustedNetYearSalaryInternal = AdjustedNetYearSalaryIntern / 12;

                // Zuordnung der berechneten Werte zu den Properties
                OldCommuteCost = oldCommuteCost;
                OldCommuteCostYear = oldCommuteCostYear;

                CommuteCost = commuteCost;
                CommuteCostYear = commuteCostYear;

                TotalCost = totalCost;
                OldTotalCost = oldTotalCost;

                MonthlyWearAndTear = monthlyWearAndTear;
                OldMonthlyWearAndTear = oldMonthlyWearAndTear;

                WearAndTearYear = wearAndTearYear;
                OldWearAndTearYear = oldWearAndTearYear;

                //Monatliche Angaben (inkl. Pendel und Abnutzungskosten)
                MonthlyTotalCostWearAndTear = totalCostWearAndTear / 12;
                OldMonthlyTotalCostWearAndTear = oldTotalCostWearAndTear / 12;

                //Jährliche Angaben (inkl. Pendel und Abnutzungskosten)
                TotalCostWearAndTear = totalCostWearAndTear;
                OldTotalCostWearAndTear = oldTotalCostWearAndTear;

                AdjustedNetSalary = adjustedNetSalary;
                OldAdjustedNetSalary = oldAdjustedNetSalary;

                AdjustedSalary = adjustedSalary;
                OldAdjustedSalary = oldAdjustedSalary;

                //Nur Netto gehalt nach Radiobutton für Externes Netto
                if (UseExternalNetto)
                {
                    ChurchTax = false;
                    GrossSalary = 0;
                    NewGrossSalary = 0;
                    if (IsSachsen)
                    {
                        OldAdjustedNetYearSalary = _netSalaryCalculationService.AdjustNetSalaryForSachsen(ExternOldNetSalary, IsSachsen);
                        OldMonthlyAdjustedNetYearSalary = OldAdjustedNetYearSalary / 12;

                        AdjustedNetYearSalary = _netSalaryCalculationService.AdjustNetSalaryForSachsen(ExternNetSalary, IsSachsen);
                        MonthlyAdjustedNetYearSalary = AdjustedNetYearSalary / 12;

                        Debug.WriteLine(OldMonthlyAdjustedNetYearSalary);

                    }
                    else
                    {
                        //Alter Job bekanntes Netto
                        OldAdjustedNetYearSalary = Math.Max(0, ExternOldNetSalary - oldTotalAnnualCost);
                        OldMonthlyAdjustedNetYearSalary = OldAdjustedNetYearSalary / 12;

                        //Neuer Job bekanntes Netto
                        AdjustedNetYearSalary = Math.Max(0, ExternNetSalary - totalAnnualCost);
                        MonthlyAdjustedNetYearSalary = AdjustedNetYearSalary / 12;
                    }  
                }

            }

            // Optional: Ausgabe in der View , Brutto nach Abzügen
            GrossAfterDeduction = grossAfterDeductions;
            OldGrossAfterDeduction = oldGrossAfterDeductions;

            MonthlyGrossAfterDeduction = GrossAfterDeduction / 12;
            OldMonthlyGrossAfterDeduction = OldGrossAfterDeduction / 12;

            IsCalculated = true;
        }

        //Hilfsmethode für Kirchensteuerrate
        private float GetChurchTaxRate(bool churchTax, string bundesland)
        {
            return _netSalaryCalculationService.CalculateChurchTaxRate(ChurchTax, Bundesland);
        }

        //alter job ist mobile neuer job ist nicht mobile
        private void CalculateForOldMobileNewNonMobile()
        {

            OldCommuteDistance = 0;
            OldCommuteDurationManual = 0;
            OldAverageCommuteDays = 0;
            OldFormattedCommuteDuration = "Keine Pendelzeit";
            OldCommuteCost = 0;
            OldCommuteCostYear = 0;

            // neuer job berechnung der pendelstrecke und dauer automatisch
            if (UseCalculateDistance)
            {
                var result = CommuteCalculationHelper.CalculateForOldMobileNewNonMobile(CommuteDistance);

                FullDistance = DistanceData.FullDistance;
                CommuteDistance = DistanceData.Distance;
                FormattedCommuteDuration = result.FormattedDuration;
            }
            else
            {
                //Manuelle eingabe verwenden
                CommuteDistance = CommuteDistanceManual;
                FormattedCommuteDuration = ConvertMinutesToTime(CommuteDurationManual);
            }
        }


        private void CalculateForOldNonMobileNewMobile()
        {
            CommuteDistance = 0;
            CommuteDurationManual = 0;
            FormattedCommuteDuration = "Keine Pendelzeit";
            CommuteCost = 0;
            CommuteCostYear = 0;
            //automatische berechnung für distanz alter job
            if (UseOldCalculateDistance)
            {
                OldFullDistance = DistanceData.OldFullDistance;
                OldCommuteDistance = DistanceData.OldDistance;

                var result = CommuteCalculationHelper.CalculateForNewMobileOldNonMobile(OldCommuteDistance);
                OldFormattedCommuteDuration = result.OldFormattedDuration;
            }
            else
            {
                //manuelle eingabe
                OldCommuteDistance = OldCommuteDistanceManual;
                OldFormattedCommuteDuration = ConvertMinutesToTime(OldCommuteDurationManual);
            }
        }



        private void CalculateForBothNonMobile()
        {
            if (!UseCalculateDistance && !UseOldCalculateDistance)
            {
                var result = CommuteCalculationHelper.CalculateForBothNonMobile(CommuteDistanceManual, OldCommuteDistanceManual);

                CommuteDistance = result.CommuteDistance;
                OldCommuteDistance = result.OldCommuteDistance;
                CommuteDurationManual = result.CommuteDuration;
                OldCommuteDurationManual = result.OldCommuteDuration;
                FormattedCommuteDuration = result.FormattedDuration;
                OldFormattedCommuteDuration = result.OldFormattedDuration;
            }
        }

        private void SetNoCommuteForBoth()
        {
            var result = CommuteCalculationHelper.SetNoCommuteForBoth();
            CommuteDistance = result.CommuteDistance;
            CommuteDurationManual = result.CommuteDuration;
            AverageCommuteDays = (int)result.AverageCommuteDays;
            FormattedCommuteDuration = result.FormattedDuration;

            OldCommuteDistance = result.CommuteDistance;
            OldCommuteDurationManual = result.CommuteDuration;
            OldAverageCommuteDays = (int)result.AverageCommuteDays;
            OldFormattedCommuteDuration = result.FormattedDuration;
        }

        private void ApplyAutomaticDistanceValues()
        {
            // Wenn die berechneten Werte 0 sind, verwende die gespeicherten Werte aus DistanceData (Automatisch)
            if (FullDistance == 0 || CommuteDistance == 0)
            {
                if (DistanceData.FullDistance != 0 && DistanceData.Distance != 0)
                {
                    FullDistance = DistanceData.FullDistance;
                    CommuteDistance = DistanceData.Distance;
                }
                else
                {
                    // Wenn auch DistanceData ungültig ist, dann zeige eine Fehlermeldung an
                    ModelState.AddModelError("", "Die Distanzwerte sind ungültig.");
                }
            }
        }
        private static string ConvertMinutesToTime(float minutes)
        {
            int hours = (int)minutes / 60;
            int mins = (int)minutes % 60;
            return $"{hours}h {mins}m";
        }

        private void ApplyOldAutomaticDistanceValues()
        {
            if (OldFullDistance == 0 || OldCommuteDistance == 0)
            {
                if (DistanceData.OldFullDistance != 0 && DistanceData.OldDistance != 0)
                {
                    OldFullDistance = DistanceData.OldFullDistance;
                    OldCommuteDistance = DistanceData.OldDistance;
                }
                else
                {
                    ModelState.AddModelError("", "Die Distanzwerte sind ungültig.");
                }
            }
        }

    }
}