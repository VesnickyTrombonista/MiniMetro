## Specifikace zápočtového programu pro předměty NPRG035 + NPRG038: 
## Popis:
Mám představu, že udělám 2D hru v Unity, napodobení klasické verze hry
MiniMetro viz: https://cs.wikipedia.org/wiki/Mini_Metro

Vytvořil bych jen normální režim na jedné mapě, kde přibývají zákazníci a na stanicích se časem spustí časový limit při přeplnění. Jinak styl hry bych ponechal. 
Udělal bych jednu mapu, kde na začátku bude pomalé generování stanic a zákazníků, navíc to bude v malém okruhu. Postupem času se okruh generování nových stanic bude zvětšovat, stejně tak se bude zvětšovat i počet zákazníků a rozmanitost typů stanic. 
K samotné hře bych ještě dodal, že bych rád zachoval koncept přibývání vylepšení po každém týdnu: nová linka metra (do omezeného počtu), přestupní stanice: zvětší kapacitu stanice, nový vlak, další vagon a tunely na spojování pod řekou.
Hra se bude ovládat myší jako v originále, případně nějaké speciální klávesy pro pozastavení hry, zrychlení apod.

Cílem hry je přepravit co nejvíce zákazníků. Jde o nahrání maximálního skóre, takže hra nemá přesně vymezený konec. 
V rámci UI bych zmínil, že veškerý herní stav uvidíme na obrazovce a využití bonusů na spodní liště. Skóre a čas bych zobrazil spíše nahoře.

Jde vlastně o diskrétní simulaci, takže bych rád využil paralelismus na zpracování požadavků od UI a simulaci při generování, např. spojování stanic metra novou linkou a generování dalších stanic.


##### další info:
Specifikace mít do 7. 7. 2023

##### Předvedení finální plně funkční verze (včetně uživatelské a programátorské dokumentace):
2. deadline: 8. 9. 2023: Předvedeno do 2. deadline: minimálně 60 kB zdrojového kódu v jazyce C#