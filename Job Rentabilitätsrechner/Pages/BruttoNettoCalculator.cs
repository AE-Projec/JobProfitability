namespace Job_Rentabilitätsrechner.Pages
{
    public class BruttoNettoCalculator
    {
        public float CalculateNetto(float brutto, int steuerklasse, bool kirchensteuer)
        {
            float lohnsteuer = CalculateLohnsteuer(brutto, steuerklasse);
            float solidaritätszuschlag = lohnsteuer * 0.055f;
            float kirchensteuerbetrag = kirchensteuer ? lohnsteuer * 0.09f : 0f;

            float rentenversicherung = brutto * 0.093f; //9,3 % für Rentenversicherung
            float krankenversicherung = brutto * 0.073f;// 7,3% für Krankenversicherung
            float pflegeversicherung = brutto * 0.01525f; // 1,525% für Pflegeversicherung
            float arbeitslosenversicherung = brutto * 0.012f; // 1,2% für Arbeitslosenversicherung

            float netto = brutto - (lohnsteuer + kirchensteuerbetrag +
                                      rentenversicherung + krankenversicherung +
                                      pflegeversicherung + arbeitslosenversicherung);

            return netto;
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

            // Steuerfreibeträge und Sonderregelungen für verschiedene Steuerklassen
            switch (steuerklasse)
            {
                case 1:
                    grundfreibetrag = 10908f; // für Steuerklasse 1 
                    break;
                case 2:
                    grundfreibetrag = 10908f;
                    entlastungsbetragAlleinerziehende = 4260f; // Entlastungsbetrag für Alleinerziehende
                    break;
                case 3:
                    grundfreibetrag = 20000f; // Beispielwert für verheiratete Steuerklasse 3
                    break;
                case 4:
                    grundfreibetrag = 10908f; // für Steuerklasse 4
                    break;
                case 5:
                    grundfreibetrag = 0f; // Steuerklasse 5 hat keine Freibeträge
                    break;
                case 6:
                    grundfreibetrag = 0f; // Steuerklasse 6 hat keine Freibeträge
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
    }
}
