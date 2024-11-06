namespace Job_Rentabilitätsrechner.Interfaces
{
    public interface INetSalaryCalculationService
    {
        // Berechnet das Nettogehalt
        float CalculateNetSalary(float grossSalary, int taxClass, bool churchTax, float kirchensteuerRate, bool isSachsen);

        // Berechnet das neue Nettogehalt
        //float CalculateNewNetSalary(float newGrossSalary, int taxClass, bool churchTax, float churchTaxRate, bool isSachsen);

        // Berechnet die Kirchensteuerrate
        float CalculateChurchTaxRate(bool churchTax, string state);

        // Passt den Treibstoffverbrauch an
        float AdjustFuelConsumption(float consumption, string transmissionType, int? gearCount);

        // Berechnet Nettogehälter
        void CalculateNetSalaries(float grossSalary, float newGrossSalary, int taxClass, bool churchTax, float churchTaxRate,
            bool useExternalNetto,bool isSachsen ,float? externalOldNetSalary, float? externalNewNetSalary, out float netSalary, out float newNetSalary);

        // Überprüft, ob der Solidaritätszuschlag angewendet wird
        bool ShouldApplySoli(float grossSalary, int taxClass);

        // Berechnet die Lohnsteuer
        float CalculateLohnsteuer(float grossSalary, int steuerklasse);

        // Berechnet das Bruttogehalt nach Abzügen
        float CalculateGrossAfterDeductions(float brutto, int steuerklasse, bool kirchensteuer, float kirchensteuerRate = 0.09f);

        // Berechnet den Solidaritätszuschlag für externes Nettogehalt
        float CalculateSoliForExternalNetto(float externalNetSalary, int taxClass);

        bool CheckSoliThreshold(float grossSalary, int taxClass);

        public float AdjustNetSalaryForSachsen(float externalNetSalary, bool isSachsen);
    }

}

