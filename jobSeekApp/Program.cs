using System.Collections;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using SerpApi;
using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        string apiKey = "5715b5ba303ace17236bbdb7db24d6d9065afc020e008229eb0b890653731f7b"; // à adapter
        Hashtable ht = new Hashtable
        {
            {"engine", "google_jobs"},
            {"google_domain", "google.fr"},
            {"hl", "fr"},
            {"gl", "fr"},
            {"location", "France"}
        };

        string emploi = "Développeur Java";
        ht.Add("q", emploi);

        string chemin_fichier = "C:\\Users\\dasyx\\Desktop\\offres_emploi.txt"; //à adapter

        if (!File.Exists(chemin_fichier))
        {
            File.Create(chemin_fichier).Close();
        }

        if (OperatingSystem.IsWindows())
        {
            RegistryKey? rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (rk != null)
            {
                rk.SetValue("Recherche de Job", System.Reflection.Assembly.GetExecutingAssembly().Location);
                // rk.DeleteValue("Recherche de Job",false);
            }
        }

        try
        {
            GoogleSearch search = new GoogleSearch(ht, apiKey);
            JObject data = search.GetJson();
            JArray results = (JArray)data["jobs_results"];

            string resultats_actuels = "";
            using (StreamReader sr = new StreamReader(chemin_fichier))
            {
                resultats_actuels = sr.ReadToEnd();
            }

            Console.WriteLine("Recherche d'offres d'emploi " + emploi + " en cours...");
            foreach (JObject result in results)
            {
                string? lien = (string) result["related_links"][0]["link"];
                string? job_id = (string)result["job_id"];
                string? titre_poste = (string)result["title"];
                string? company_name = (string)result["company_name"];
                string? location = (string)result["location"];
                string? via = (string)result["via"];
                string? description = (string)result["description"];

                if (job_id != null && !resultats_actuels.Contains(job_id))
                {
                    Console.WriteLine("Nouvelle offre d'emploi trouvée : " + titre_poste + " à " + company_name + " (" + location + ")");
                    Console.WriteLine("Via: " + via);
                    Console.WriteLine("Description: " + description);
                    Console.WriteLine("Lien: " + lien);
                    Console.WriteLine();
                    Console.WriteLine(new string('-', 50)); // Ligne séparatrice
                    Console.WriteLine();

                    using (StreamWriter sw = new StreamWriter(chemin_fichier, true))
                    {
                        sw.WriteLine(new string('-', 50)); // Ligne séparatrice dans le fichier
                        sw.WriteLine();
                        sw.WriteLine("Titre: " + titre_poste);
                        sw.WriteLine("Entreprise: " + company_name);
                        sw.WriteLine("Localisation: " + location);
                        sw.WriteLine("Via: " + via);
                        sw.WriteLine("Description: " + description);
                        sw.WriteLine("Lien: " + lien);
                        sw.WriteLine();
                    }
                }
            }
        }
        catch (SerpApiSearchException ex)
        {
            Console.WriteLine("Exception:");
            Console.WriteLine(ex.ToString());
        }

        Console.WriteLine("Recherche terminée.");
        Console.ReadLine();
    }
}



