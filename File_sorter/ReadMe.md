# File Sorter

**Autor:** Ondřej Švéda

## 1. Popis projektu
File Sorter je konzolová aplikace navržená pro **paralelní třídění velkého množství souborů**. Hlavním cílem projektu bylo efektivně využít vícevláknové programování (Tasks) k překonání **I/O bottlenecku** při přesouvání dat na disku.

---

## 2. Architektonické principy
Jádro aplikace je postaveno na vzoru **Producer-Consumer**.

* **Producent (metoda `Produce`):** Rychle skenuje zdrojový adresář a vkládá cesty souborů do fronty.
* **Konzumenti (metoda `Consume`):** Více paralelních vláken (počet je konfigurovatelný) odebírá položky z fronty a provádí pomalou I/O operaci přesunu souboru.
* **Fronta (`BlockingCollection<string>`):** Slouží jako bezpečný, synchronizovaný mechanismus pro předání práce.

### Struktura kódu (Namespace: `File_sorter`)

| Třída | Účel                                                                                                        |
| :--- |:------------------------------------------------------------------------------------------------------------|
| **`Program.cs`** | **Vstupní bod.** Pouze načte konfiguraci a spustí třídu `Organizer`.                                        |
| **`Organizer.cs`** | **Jádro logiky.** Zapouzdřuje metody `Produce` a `Consume`, řídí cyklus vláken.                             |
| **`AppConfig.cs`** | **Konfigurace.** Načítá `settings.txt` a řeší relativní cesty, a to přímo z **kořenové složky projektu**.   |
| **`Logger.cs`** | **Thread-safe Logování.** Zajišťuje bezpečný zápis do souboru i z více vláken pomocí **`lock`** mechanismu. |

---

## 3. Konfigurace a Spuštění

Aplikace je plně konfigurovatelná přes externí soubor. Bylo opraveno načítání konfigurace, která se nyní hledá v kořenovém adresáři projektu.

### Soubor: `config/settings.txt`

```ini
# Cesty se načítají relativně ke složce s .csproj (kořen projektu)
SourcePath=./Input       
TargetPath=./Output     
# Počet paralelně pracujících vláken
ThreadCount=3           
