namespace Job_Rentabilitätsrechner.Pages
{
    public class BruttoNettoCalculator
    {
        public float CalculateNetto(float brutto, int steuerklasse, bool kirchensteuer)
        {
            float lohnsteuer = CalculateLohnsteuer(brutto, steuerklasse);
            float solidaritätszuschlag = lohnsteuer * 0.055f;
            float kirchensteuerbetrag = kirchensteuer ? lohnsteuer * 0.09f : 0f;

            float rentenversicherung = brutto * 0.093f;
            float krankenversicherung = brutto * 0.073f;
            float pflegeversicherung = brutto * 0.01525f;
            float arbeitslosenversicherung = brutto * 0.012f;

            float netto = brutto - (lohnsteuer + solidaritätszuschlag + kirchensteuerbetrag +
                                      rentenversicherung + krankenversicherung +
                                      pflegeversicherung + arbeitslosenversicherung);

            return netto;
        }
        private float CalculateLohnsteuer(float brutto, int steuerklasse)
        {
            float steuer = 0f;

            if (steuerklasse == 1)
            {
                steuer = brutto * 0.2f;
            }
            else if (steuerklasse == 3)
            {
                steuer = brutto * 0.15f;
            }
            else if (steuerklasse == 5)
            {
                steuer = brutto * 0.25f;
            }

            return steuer;
        }
    }
}
