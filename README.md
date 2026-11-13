# Gestion des biens immobiliers (POO – Python)

Mini-projet de conception et programmation orientée objet : suivi des locations, contrats, loyers et maintenance.

## 1. Diagramme de cas d'utilisation général

```mermaid
flowchart LR
    GES(["Gestionnaire / Agence"])
    PRO(["Propriétaire"])
    LOC(["Locataire"])
    TEC(["Technicien"])

    subgraph SYS["Système de gestion des biens immobiliers"]
        UC1["Gérer les biens"]
        UC2["Gérer les propriétaires et locataires"]
        UC3["Créer / résilier un contrat"]
        UC4["Générer l'échéancier des loyers"]
        UC5["Enregistrer un paiement"]
        UC6["Émettre une quittance"]
        UC7["Relancer les impayés"]
        UC8["Signaler une panne"]
        UC9["Traiter une demande de maintenance"]
        UC10["Consulter le tableau de bord"]
        UC11["Consulter ses biens et revenus"]
        UC12["Consulter ses loyers et quittances"]
    end

    GES --> UC1
    GES --> UC2
    GES --> UC3
    GES --> UC5
    GES --> UC7
    GES --> UC9
    GES --> UC10
    UC3 -. include .-> UC4
    UC5 -. extend .-> UC6
    PRO --> UC11
    PRO --> UC10
    LOC --> UC12
    LOC --> UC8
    TEC --> UC9
```

## 2. Diagramme de classes

```mermaid
classDiagram
    class Personne {
        <<abstract>>
        +int id
        +str nom
        +str prenom
        +str email
        +str telephone
        +nom_complet() str
        +role() str*
    }
    class Proprietaire {
        +list biens
        +ajouter_bien(bien)
        +revenus_potentiels() float
    }
    class Locataire {
        +str profession
        +list contrats
        +contrats_actifs() list
    }
    class Technicien {
        +str specialite
        +list interventions
        +charge_de_travail() int
    }
    class Bien {
        <<abstract>>
        +int id
        +str adresse
        +str ville
        +float surface
        +float loyer_base
        +StatutBien statut
        +type_bien() str*
        +est_disponible() bool
        +changer_statut(statut)
        +cout_maintenance_total() float
        +loyer_au_m2() float
    }
    class Appartement {
        +int etage
        +bool ascenseur
    }
    class Maison {
        +float surface_jardin
        +bool garage
    }
    class LocalCommercial {
        +str activite
    }
    class Contrat {
        +int id
        +date date_debut
        +date date_fin
        +float loyer_mensuel
        +float depot_garantie
        +StatutContrat statut
        +est_actif() bool
        +jours_restants() int
        +generer_echeancier() list
        +reviser_loyer(taux) float
        +loyers_impayes() list
        +resilier()
        +terminer()
    }
    class Loyer {
        +int id
        +date mois
        +float montant
        +date date_echeance
        +StatutLoyer statut
        +enregistrer_paiement(montant, date, mode) Paiement
        +mettre_a_jour_statut() StatutLoyer
        +penalite() float
        +generer_quittance() Quittance
    }
    class Paiement {
        +int id
        +float montant
        +date date_paiement
        +ModePaiement mode
    }
    class Quittance {
        +int id
        +date date_emission
        +texte() str
    }
    class DemandeMaintenance {
        +int id
        +str description
        +Priorite priorite
        +StatutMaintenance statut
        +float cout
        +assigner(technicien)
        +demarrer()
        +terminer(cout)
        +annuler()
    }
    class GestionImmobiliere {
        +str nom_agence
        +ajouter_bien(bien, proprietaire)
        +creer_contrat(...) Contrat
        +signaler_panne(...) DemandeMaintenance
        +taux_occupation() float
        +loyers_impayes() list
        +contrats_a_echeance(jours) list
        +tableau_de_bord() dict
    }

    Personne <|-- Proprietaire
    Personne <|-- Locataire
    Personne <|-- Technicien
    Bien <|-- Appartement
    Bien <|-- Maison
    Bien <|-- LocalCommercial

    Proprietaire "1" --> "0..*" Bien : possède
    Locataire "1" --> "0..*" Contrat : signe
    Bien "1" --> "0..*" Contrat : fait l'objet de
    Contrat "1" *-- "1..*" Loyer : échéancier
    Loyer "1" *-- "0..*" Paiement : réglé par
    Loyer "1" --> "0..1" Quittance : génère
    Bien "1" --> "0..*" DemandeMaintenance : concerne
    Technicien "0..1" --> "0..*" DemandeMaintenance : intervient
    GestionImmobiliere o-- Bien
    GestionImmobiliere o-- Contrat
    GestionImmobiliere o-- Proprietaire
    GestionImmobiliere o-- Locataire
    GestionImmobiliere o-- Technicien
    GestionImmobiliere o-- DemandeMaintenance
```

Les mêmes diagrammes sont fournis en PlantUML : `diagramme_classes.puml` et `cas_utilisation.puml`.

## 3. Structure du code

| Fichier | Contenu |
|---|---|
| `enums.py` | Statuts, priorités, modes de paiement |
| `personnes.py` | `Personne` (abstraite), `Proprietaire`, `Locataire`, `Technicien` |
| `biens.py` | `Bien` (abstraite), `Appartement`, `Maison`, `LocalCommercial` |
| `contrat.py` | `Contrat` (échéancier, révision, résiliation) |
| `finances.py` | `Loyer`, `Paiement`, `Quittance` |
| `maintenance.py` | `DemandeMaintenance` |
| `gestion.py` | `GestionImmobiliere` (façade + statistiques) |
| `main.py` | Démonstration |
| `test_immobilier.py` | Tests unitaires |

Concepts POO utilisés : encapsulation, héritage (`Personne`, `Bien`), classes abstraites, polymorphisme (`role()`, `type_bien()`), composition (`Contrat` → `Loyer` → `Paiement`) et agrégation (`GestionImmobiliere`).

## 4. Exécution

```bash
python main.py
python -m unittest -v
```
