### MiniMetro – Josef Bálek
#### Programátorská dokumentace:

###### Info

Hodně programátorské dokumentace je součástní kódu, u funkcí jsou dokumetnační komentářa a občas jsou vysvětleny i nějaké záhadné věci. Proto mi nepřijde užitečné, abych vše psal sem, kód je přiložen.

###### Skripty a o nich

TimePlanning.cs - dá se říct, že hlavní skript, který řídí a plánuje vše, co se času týče

StationGenerating.cs - generuje stanice a nastavuje jim zásadní vlastnosti, přidán náhodný generátor

PeopleGenerating.cs - generuje lidi u dané stanice

LinesDrawer.cs - pomáhá kreslit linku

TrainDeletor.cs - má na starosti, aby kliknutí myši při výběru linky nevnímal jako novou linku a smaže nepovedený pokus

Line.cs - uchovává informace o lince a zajišťuje si vlastní vlaky metra

Station.cs - obsahuje informace o sobě a zajišťuje generování lidí

Person.cs - jen drží informace o člověku

ButtonManager.cs - správce tlačítek, zajišťuje překlikávání linek

TrainsSpeed.cs - skript na tlačítko, které týdenně přidává rychlost vlakům

Train.cs - informace o vlaku, koho přepravuje a zajišťuje chování při příjezdu do stanice

Celkově tyto skripty mají 62,2 kB.

###### Vícevláknovost
Ta je využita při generování stanic metra, protože ne všechny funkce na generování jsou jednoduché. Dále by stejná technika šla použít i u generování lidí, ale to není tak složité. Poslední věc je vlaku, když projíždí stanice. Tam by bylo dobré si zamykat a list psažérů, aby do něj nikdo nezasahoval.

###### Unity

Další věc jsou vygenerovaná data pro projekt v Unity, bez kterých by se to neobešlo. V Unity je také dost nastavování v Inspektoru, ale to jsem do samotného kódu neuvažoval.

###### Bugy

Tento projekt není plně odladěn, takže je problém, při pomalém natahování linek. Zároveň nefunguje perfektně přesouvání lidí ze stanice do metra.
Co se týče škálování, tak pozice metra je škálována, ale mohla by být i lépe usazena k lince.

Trochu větší závada je u generování stanic. I přes četné kontroly se mi nepovedlo rovnoměrně vyvážit poměr geneorvání vzhledem ke vzdálenosti od středu.

###### Vylepšení

Drobné vylepšení by mohlo být přidání menšího skriptu na hudbu do hry.

Zároveň, pokud by se časem přidaly levely, je možné přidat hlavní menu, které by mohlo být propracované.

Dalšími vylepšeními jsou různé herní vychytávky a upgrady, které se můžou generovat každý herní týden a přidat rozšíření o hru nad vodou, u které mi nefungoval collider, možná je jen špatně nastaven.
A celkově do originálu by bylo potřeba přidat a odladit spoustu věcí.