using System.Text;
using GestionBiensImmobiliers;

Console.OutputEncoding = Encoding.UTF8;

// "dotnet run -- test" lance les tests unitaires.
if (args.Length > 0 && args[0] == "test")
{
    return Tests.Lancer();
}

var agence = new GestionImmobiliere("Agence Médina Immo");

var prop = agence.AjouterProprietaire(new Proprietaire("Ben Ali", "Karim", "karim@mail.tn", "20111222"));
var loc1 = agence.AjouterLocataire(new Locataire("Trabelsi", "Sana", "sana@mail.tn", "55123456", "Ingénieure"));
var loc2 = agence.AjouterLocataire(new Locataire("Gharbi", "Omar", "omar@mail.tn", "98765432", "Commerçant"));
var tech = agence.AjouterTechnicien(new Technicien("Jlassi", "Mehdi", "mehdi@mail.tn", "22334455", "Plomberie"));

var appart = agence.AjouterBien(new Appartement("12 rue de la Liberté", "Tunis", 85, 900, etage: 3, ascenseur: true), prop);
var maison = agence.AjouterBien(new Maison("5 avenue des Jasmins", "La Marsa", 150, 1800, surfaceJardin: 200, garage: true), prop);
agence.AjouterBien(new LocalCommercial("8 rue de Marseille", "Tunis", 40, 1200, activite: "Boutique"), prop);

var c1 = agence.CreerContrat(appart, loc1, new DateTime(2026, 1, 1), new DateTime(2026, 12, 31), 1800);
agence.CreerContrat(maison, loc2, new DateTime(2026, 6, 1), new DateTime(2027, 5, 31), 3600);

// Paiements
foreach (var loyer in c1.Loyers.Take(10))
    loyer.EnregistrerPaiement(loyer.Montant, loyer.DateEcheance, ModePaiement.Virement);
Console.WriteLine(c1.Loyers[0].GenererQuittance().Texte());
Console.WriteLine();

// Maintenance
var demande = agence.SignalerPanne(appart, "Fuite sous l'évier", Priorite.Haute);
demande.Assigner(tech);
demande.Demarrer();
demande.Terminer(120);

var aujourdhui = new DateTime(2026, 11, 13);
Console.WriteLine("Biens :");
foreach (var b in agence.Biens)
    Console.WriteLine(" - " + b);

Console.WriteLine($"\nTableau de bord au {aujourdhui:yyyy-MM-dd}");
foreach (var kv in agence.TableauDeBord(aujourdhui))
    Console.WriteLine($" {kv.Key}: {kv.Value}");

return 0;
