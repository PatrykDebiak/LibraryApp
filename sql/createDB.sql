CREATE TABLE Authors (
                         AuthorID SERIAL PRIMARY KEY,
                         Name TEXT NOT NULL,
                         BirthDate DATE NOT NULL
);

CREATE TABLE Books (
                       BookID SERIAL PRIMARY KEY,
                       Title TEXT NOT NULL,
                       AuthorID INT REFERENCES Authors(AuthorID),
                       PublishedYear INT NOT NULL,
                       Available BOOLEAN NOT NULL
);
