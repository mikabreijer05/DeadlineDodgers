# Deadline Dodgers — Project Portfolio

## Over dit project

Dit project is ontwikkeld door **Deadline Dodgers** als onderdeel van een schoolopdracht binnen Interface Development. Het doel van dit project was om een geïntegreerde digitale oplossing te ontwerpen en ontwikkelen voor **Matrix Inc.**, een fictief bedrijf dat mechanische onderdelen verkoopt aan zakelijke en particuliere klanten.

Matrix Inc. had behoefte aan één centraal systeem dat het volledige proces ondersteunt: van het bekijken en bestellen van producten tot het beheren en bezorgen van bestellingen.

Het eindresultaat bestaat uit drie samenwerkende applicaties met een centrale database:

- **Webshop** voor klanten
- **Backoffice** voor beheerders
- **Mobiele bezorgersapp** voor bezorgers

Dit project is bedoeld als portfolio- en demonstratieproject. De repository laat zien hoe wij als team een softwareoplossing hebben geanalyseerd, ontworpen, ontwikkeld en getest.

---

## De opdracht

De uitdaging was om verschillende gebruikersgroepen te ondersteunen binnen één samenhangend systeem.

### Klanten
Klanten moeten producten eenvoudig kunnen bekijken, zoeken en bestellen.

### Beheerders
Beheerders moeten producten, gebruikers, bestellingen en leveringen kunnen beheren zonder rechtstreeks in de backend of database te hoeven werken.

### Bezorgers
Bezorgers moeten tijdens hun werkzaamheden inzicht hebben in toegewezen leveringen en deze kunnen verwerken en afronden.

Daarom is het project opgesplitst in drie applicaties die samenwerken met dezelfde centrale backend en database.

---

# Wat we hebben gemaakt

## 1. Webshop

De klantgerichte webapplicatie maakt het mogelijk om onder andere:

- een account aan te maken en in te loggen;
- producten en categorieën te bekijken;
- producten te zoeken en filteren;
- producten aan een winkelwagen toe te voegen;
- bestellingen te plaatsen;
- de status en geschiedenis van bestellingen te bekijken;
- reviews bij producten te plaatsen.

Bij het ontwerp lag de nadruk op een duidelijke navigatiestructuur en een gebruiksvriendelijke interface.

---

## 2. Admin Backoffice

Voor beheerders hebben we een aparte administratieve applicatie ontwikkeld.

Hiermee kunnen beheerders onder andere:

- producten toevoegen, wijzigen en verwijderen;
- product- en categoriegegevens beheren;
- gebruikers beheren;
- bestellingen bekijken en verwerken;
- orders filteren en sorteren;
- leveringen aanmaken, bekijken, wijzigen en verwijderen;
- aangeven wanneer een bestelling klaar is voor verzending.

De backoffice is ontwikkeld om administratieve processen overzichtelijker te maken en gebruikers in staat te stellen gegevens te beheren via een interface in plaats van rechtstreeks via de backend.

---

## 3. Mobiele bezorgersapp

Voor bezorgers hebben we een mobiele applicatie ontworpen en ontwikkeld die het bezorgproces ondersteunt.

De app bevat functionaliteiten voor:

- het bekijken van toegewezen en actieve leveringen;
- het bijwerken van de bezorgstatus;
- het afronden van leveringen;
- het scannen van pakketten;
- het toewijzen en controleren van voertuigen;
- het melden van voertuigschade met foto's;
- het gebruiken van GPS-functionaliteit;
- het openen van routebeschrijvingen;
- contact opnemen met de backoffice bij problemen.

---

## Centrale dataopslag

De applicaties zijn ontworpen rondom een centrale SQL-database. Hierdoor kunnen de verschillende onderdelen van het systeem met dezelfde gegevens werken.

In het ontwerp is aandacht besteed aan:

- datamodellering;
- normalisatie;
- Entity Relationship Diagrams (ERD);
- klassendiagrammen;
- consistente gegevensopslag;
- onderhoudbaarheid en toekomstige uitbreidbaarheid.

---

# Hoe we te werk zijn gegaan

Dit project is iteratief uitgevoerd en bestond grofweg uit vier fasen.

## Analyse

We hebben het probleem en de verschillende gebruikersgroepen onderzocht met onder andere:

- de **5W + 1H-methode**;
- een **Current Reality Tree (CRT)**;
- stakeholder- en doelgroepanalyses;
- interviews;
- analyses van bestaande oplossingen.

Op basis hiervan hebben we functionele en niet-functionele requirements opgesteld.

## Ontwerp

Voor het ontwerp hebben we gewerkt met:

- requirements;
- use cases;
- use-case modellen;
- activity diagrams;
- klassendiagrammen;
- low-fidelity prototypes;
- Figma-designs;
- HCI-principes;
- Gestaltprincipes;
- databaseontwerp en normalisatie.

## Realisatie

Vervolgens hebben we de belangrijkste onderdelen van de drie applicaties gerealiseerd als prototypes en werkende software.

De gebruikte technologieën binnen het project omvatten onder andere:

- **ASP.NET Razor Pages**
- **ASP.NET Core MVC**
- **.NET MAUI**
- **SQL-database**

## Testen en verbeteren

De functionaliteit en gebruiksvriendelijkheid zijn getest met:

- functionele tests;
- validatie van niet-functionele requirements;
- gebruikerstesten;
- **Thinking Aloud-sessies**.

Tijdens de Thinking Aloud-tests voerden gebruikers taken uit terwijl zij hardop vertelden wat zij verwachtten en ervoeren. Hierdoor konden we problemen in de navigatie en gebruikerservaring identificeren en verbeteringen doorvoeren.

---

# Wat we hebben geleerd

## Softwareontwikkeling als proces

Een van de belangrijkste lessen uit dit project is dat softwareontwikkeling meer is dan alleen programmeren.

We hebben geleerd om een oplossing stapsgewijs op te bouwen:

**probleem analyseren → requirements opstellen → ontwerpen → ontwikkelen → testen → verbeteren**

Door deze aanpak konden we keuzes beter onderbouwen en gericht werken aan functionaliteiten die aansluiten bij gebruikersbehoeften.

## Werken met verschillende architecturen en technologieën

Tijdens dit project hebben we ervaring opgedaan met verschillende soorten applicaties en frameworks:

- webontwikkeling met ASP.NET;
- werken met Razor Pages;
- werken volgens het MVC-pattern;
- mobiele ontwikkeling met .NET MAUI;
- werken met een centrale SQL-database.

Hierdoor hebben we geleerd hoe verschillende applicaties onderdeel kunnen zijn van één groter systeem.

## Databaseontwerp

We hebben geleerd hoe belangrijk een goed databaseontwerp is wanneer meerdere applicaties dezelfde gegevens gebruiken.

Daarbij hebben we gewerkt met:

- relaties tussen gegevens;
- normalisatie;
- dataconsistentie;
- klassendiagrammen;
- ERD's;
- centrale gegevensopslag.

## Requirements en gebruikersgericht ontwikkelen

We hebben geleerd om niet direct vanuit technische oplossingen te denken, maar eerst te onderzoeken wat gebruikers daadwerkelijk nodig hebben.

Door requirements, interviews, analyses en gebruikerstests te combineren, konden we onze keuzes beter baseren op gebruikersbehoeften.

## UX en interfaceontwerp

Tijdens het ontwerpen hebben we geleerd hoe belangrijk duidelijke interactie en navigatie zijn.

We hebben gewerkt met:

- low-fidelity en high-fidelity prototypes;
- feedback na gebruikersinteracties;
- affordance;
- consistente navigatie;
- HCI-principes;
- Gestaltprincipes.

Ook hebben we ervaren dat iets wat voor ontwikkelaars logisch lijkt, voor een gebruiker niet altijd vanzelfsprekend is.

## Testen met echte gebruikers

Door Thinking Aloud-sessies hebben we geleerd om gebruikersfeedback actief te gebruiken in het ontwikkelproces.

Deze tests hielpen ons om:

- onduidelijke onderdelen te herkennen;
- problemen in navigatie te ontdekken;
- verwachtingen van gebruikers beter te begrijpen;
- ontwerpen iteratief te verbeteren.

## Samenwerken aan één softwareproject

Als team hebben we ervaring opgedaan met het verdelen van werkzaamheden en het samenbrengen van verschillende onderdelen tot één geheel.

Dit project heeft ons laten zien hoe belangrijk communicatie, consistente afspraken en een gezamenlijke technische structuur zijn wanneer meerdere ontwikkelaars aan één systeem werken.

## Security en privacy

Tijdens het project is ook aandacht besteed aan:

- privacy en AVG;
- dataminimalisatie;
- toegangscontrole;
- veilige gegevensopslag;
- netwerkbeveiliging;
- tweefactorauthenticatie als beveiligingsmaatregel;
- verantwoord gebruik van AI.

Voor ontwerp, ontwikkeling en testen is gewerkt met fictieve gegevens (mock data), zodat er geen privacygevoelige bedrijfs- of persoonsgegevens in AI-tools of prototypes werden gebruikt.

---

# Toekomstige uitbreidingen

Door de beschikbare projecttijd zijn sommige functionaliteiten bewust buiten de scope gebleven. Mogelijke toekomstige uitbreidingen zijn:

- geïntegreerde betalingsfunctionaliteit;
- uitgebreider voorraadbeheer;
- uitgebreidere productfilters;
- favorietenfunctionaliteit;
- flexibelere bezorgadressen;
- verdere automatisering van het aanmaken van verzendingen;
- geïntegreerde routeplanning;
- uitgebreidere gebruikerstesten en iteraties.

---

# Portfolio-doel

Deze repository wordt gedeeld als onderdeel van ons portfolio. Het project geeft inzicht in onze ervaring met:

- softwareontwikkeling;
- webontwikkeling;
- mobiele ontwikkeling;
- databaseontwerp;
- softwarearchitectuur;
- requirements engineering;
- UX/UI-ontwerp;
- gebruikersonderzoek;
- testen en validatie;
- iteratief verbeteren;
- samenwerken binnen een softwareproject.

De code in deze repository is een resultaat van een schoolproject en is bedoeld om potentiële stagebedrijven en andere geïnteresseerden een beeld te geven van onze technische en professionele ontwikkeling.
