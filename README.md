#  LibraryApp – Aplikacja WPF do zarządzania książkami i autorami

##  Opis aplikacji

**LibraryApp** to aplikacja desktopowa stworzona w technologii **WPF (Windows Presentation Foundation)** w języku **C#**, umożliwiająca:

- przeglądanie książek i autorów,
- dodawanie nowych książek i autorów do bazy danych,
- usuwanie książek z bazy i interfejsu użytkownika.

Interfejs graficzny został podzielony na dwie części:
- panel formularzy po lewej stronie (dodawanie książek i autorów),
- tabela z książkami po prawej stronie.

## Struktura bazy danych

Baza danych PostgreSQL o nazwie `DBLibrary` zawiera dwie główne tabele:

### Tabela `Authors`
| Kolumna     | Typ danych   | Opis                        |
|-------------|--------------|-----------------------------|
| AuthorID    | SERIAL (PK)  | Unikalny identyfikator      |
| Name        | TEXT         | Imię i nazwisko autora      |
| BirthDate   | DATE         | Data urodzenia autora       |

### Tabela `Books`
| Kolumna       | Typ danych   | Opis                        |
|---------------|--------------|-----------------------------|
| BookID        | SERIAL (PK)  | Unikalny identyfikator      |
| Title         | TEXT         | Tytuł książki               |
| AuthorID      | INT (FK)     | Klucz obcy do `Authors`     |
| PublishedYear | INT          | Rok wydania                 |
| Available     | BOOLEAN      | Czy książka jest dostępna   |

Relacja: **Books.AuthorID** → **Authors.AuthorID**

---

## Konfiguracja środowiska

### Wymagania:

- .NET 6.0 lub nowszy
- PostgreSQL (np. wersja 14 lub wyższa)
- Visual Studio 2022 lub inny IDE obsługujący WPF
- Biblioteka `Npgsql` (dla obsługi PostgreSQL)

### Instalacja biblioteki Npgsql:
W `Package Manager Console` wpisz:

```bash
Install-Package Npgsql
