using System.Formats.Asn1;

namespace Job_Rentabilitätsrechner.Pages
{
    public class NetSalaryCalculationService
    {
        private readonly BruttoNettoCalculator _calculator;

        public NetSalaryCalculationService()
        {
            _calculator = new BruttoNettoCalculator();
        }
        //berechnet brutto gehalt
        public float CalculateNetSalary(float grossSalary, int taxClass, bool churchTax, float churchTaxRate)
        {
            return _calculator.CalculateNetto(grossSalary, taxClass, churchTax, churchTaxRate);
        }

        //berechnet neues brutto gehalt
        public float CalculateNewNetSalary(float newGrossSalary, int taxClass, bool churchTax, float churchTaxRate)
        {
            return _calculator.CalculateNetto(newGrossSalary, taxClass, churchTax, churchTaxRate);
        }

        //berechnet die Kirchensteuerrate
        public float CalculateChurchTaxRate(bool churchTax, string state)
        {
            float rate = 0.09f;
            if (churchTax)
            {
                if (state == "bayern" || state == "bawue")
                {
                    rate = 0.08f;
                }
            }
            return rate;
        }


        //berechnet den Treibstoffverbrauch
        public float AdjustFuelConsumption(float consumption, string transmissionType, int? gearCount)
        {
            float adjustedConsumption = consumption;

            if (transmissionType == "automatic")
            {
                adjustedConsumption *= 1.10f; // Increase by 10% for automatic transmission

                if (gearCount.HasValue && gearCount > 5)
                {
                    adjustedConsumption *= 0.95f; // Decrease by 5% for more than 5 gears
                }
            }

            return adjustedConsumption;
        }

        public void CalculateNetSalaries(
              float grossSalary,
              float newGrossSalary,
              int taxClass,
              bool churchTax,
              float churchTaxRate,
              bool useExternalNetto,
              float? externalNetSalary,
              out float netSalary,
              out float newNetSalary) //BruttoNettoCalculator calculator
        {
            if (useExternalNetto && externalNetSalary.HasValue)
            {
                netSalary = externalNetSalary.Value;
                // ExternNetSalary = 0;
            }
            else
            {
                netSalary = CalculateNetSalary(grossSalary, taxClass, churchTax, churchTaxRate);
            }

            newNetSalary = CalculateNewNetSalary(newGrossSalary, taxClass, churchTax, churchTaxRate);

            //soli hinzufügen wenn notwendig
            if(ShouldApplySoli(grossSalary,taxClass))
            {
                netSalary = ApplySoli(netSalary);
            }
            if(ShouldApplySoli(newGrossSalary,taxClass))
            {
                newNetSalary = ApplySoli(newGrossSalary);
            }
        }

        public bool ShouldApplySoli(float grossSalary, int taxClass)
        {
            float soliThreshold = (taxClass == 3 || taxClass == 4) ? 136826f : 68413f;
            return grossSalary > soliThreshold;
        }

        private float ApplySoli(float netSalary)
        {
            return netSalary * 0.945f;
        }
    }

}
