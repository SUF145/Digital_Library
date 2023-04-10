using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{
    public enum Genre
    {
        Fiction = 1,
        NonFiction,
        Biography,
        Children
    }

    public interface ILibrary
    {
        void AddBook(Guid bookId, string title, string author, Genre genre, DateTime publicationdate, string isbn, int noOfPages);
        void CheckoutBook(Book checkBook);
        void ReturnBook(Book returnBook);
        void ViewAllBooks();
    }

    public class Book
    {
        public Guid BookId;
        public string Title;
        public string Author;
        public Genre GenreName;
        public DateTime PublicationDate;
        public string Isbn;
        public int NoOfPages;
        public DateTime BorrowedTime;
        public DateTime ReturnTime;

        public Book() { }
        public Book(Guid bookId, string title, string author, Genre genreName, DateTime publicationDate, string isbn, int noOfPages) 
        {
            this.BookId = bookId;
            this.Title = title;
            this.Author = author;
            this.GenreName = genreName;
            this.PublicationDate = publicationDate;
            this.Isbn = isbn;
            this.NoOfPages = noOfPages;
        }
    }

    public class LibraryCard
    {
        public int LibraryCardNo;
        public Guid UserId;
        public string UserName;
        public string Password;
        public List<Book> BooksBorrowed = new List<Book>();
        public List<Book> BooksReturned = new List<Book>();

        public LibraryCard(int libraryCardNo, Guid userId, string userName, string password)
        {
            this.LibraryCardNo = libraryCardNo;
            this.UserId = userId;
            this.UserName = userName;
            this.Password = password;
        }
    }

    public class LibraryManagementSystem : ILibrary
    {
        public  List<Book> Books = new List<Book>();

        public static List<Book> LendedBooks = new List<Book>();

        public static List<LibraryCard> LibraryCards = new List<LibraryCard>();

        #region Interface Implementation
        public void AddBook(Guid bookId, string title, string author, Genre genre, DateTime publicationdate, string isbn, int noOfPages)
        {
            Books.Add(new Book(bookId, title, author, genre, publicationdate, isbn, noOfPages));
        }
        public void ViewAllBooks()
        {
            var booksWithSameTitle = Books.GroupBy(book => book.Title);
            foreach (var titleGroup in booksWithSameTitle)
            {
                var booksLended = LendedBooks.Where(book => book.Title == titleGroup.Key).ToList();
                Console.WriteLine("Title : " + titleGroup.Key + "\nAuthor : " + titleGroup.FirstOrDefault().Author + "\nGenre : " + titleGroup.FirstOrDefault().GenreName +
                    "\nPublication Date : " + titleGroup.FirstOrDefault().PublicationDate.ToShortDateString() + "\nISBN : " + titleGroup.FirstOrDefault().Isbn +
                    "\nNumber of Pages : " + titleGroup.FirstOrDefault().NoOfPages + "\nQuantity : " + titleGroup.Count() + "\nLended Quantity : " + booksLended.Count() + "\n");
            }
        }

        public void CheckoutBook(Book book)
        {
            LendedBooks.Add(book);
            Books.Remove(book);
        }

        public void ReturnBook(Book returnBook)
        {
            if(LendedBooks.Count == 0) 
            {
                Books.Add(returnBook);
            }
            else
            {
                LendedBooks.Remove(returnBook);
                Books.Add(returnBook);
            }
        }
        #endregion

        #region Validation
        public static int ValidateNumber(string str)
        {
            int q;
            while (true)
            {
                Console.WriteLine(str);
                if (int.TryParse(Console.ReadLine(), out q))
                {
                    if (q > 0)
                        break;
                }
                Console.WriteLine("\nInvalid Input.\n");
            }
            return q;
        }

        public static bool ValidateIsbn(string str)
        {
            var strRegex = "ISBN(-1(?:(0)|3))?:?\\x20+(?(1)(?(2)(?:(?=.{13}$)\\d{1,5}([ -])\\d{1,7}\\3\\d{1,6}\\3(?:\\d|x)$)|(?:(?=.{17}$)97(?:8|9)([ -])\\d{1,5}\\4\\d{1,7}\\4\\d{1,6}\\4\\d$))|(?(.{13}$)(?:\\d{1,5}([ -])\\d{1,7}\\5\\d{1,6}\\5(?:\\d|x)$)|(?:(?=.{17}$)97(?:8|9)([ -])\\d{1,5}\\6\\d{1,7}\\6\\d{1,6}\\6\\d$)))";
            var re = new Regex(strRegex);
            if (re.IsMatch(str))
            {
                return true;
            }
            else
                return false;
        }

        public static bool ValidateUser(string userName, string password)
        {
            var IsUserIdPresent = LibraryCards.Where(card => card.UserName == userName && card.Password == password).FirstOrDefault();
            if (IsUserIdPresent == null)
                return false;
            else
                return true;
        }

        public static bool CheckUserName(string userName)
        {
            var IsUserNamePresent = LibraryCards.Where(card => card.UserName == userName).FirstOrDefault();
            if (IsUserNamePresent == null)
                return false;
            else
                return true;
        }

        public static string ValidateString(string displayString)
        {
            Console.WriteLine(displayString);
            var stringInput = Console.ReadLine();
            while(stringInput.Length == 0)
            {
                Console.WriteLine("\nThis field cannot be empty");
                stringInput = Console.ReadLine();
            }
            return stringInput;
        }

        /*public bool CheckIsbn(string isbn)
        {
            if (Books.Count == 0)
                return true;
            var isIsbnPresent = Books.Where(book => book.Isbn == isbn).FirstOrDefault();
            if (isIsbnPresent == null)
                return true;
            else
                return false;
        }*/

        #endregion

        #region Search Methods
        public List<Book> SearchById(string id)
        {
            var newList = Books.FindAll(book => book.BookId.ToString().Contains(id.ToLower()));
            return newList;
        }

        public List<Book> SearchByTitle(string title)
        {
            var newList = Books.FindAll(book => book.Title.ToLower().Contains(title.ToLower()));
            return newList;
        }

        public List<Book> SearchByAuthor(string author)
        {
            var newList = Books.FindAll(book => book.Author.ToLower().Contains(author.ToLower()));
            return newList;
        }

        public List<Book> SearchByGenre(Genre genre)
        {
            var foundBook = Books.Where(book => book.GenreName == genre).ToList<Book>();
            return foundBook;
        }
        #endregion

        #region Methods
        public static List<Book> ReadInputFromUser()
        {
            LibraryManagementSystem Lms = new LibraryManagementSystem();
            var title = ValidateString("Enter the Title of book");
            var author = ValidateString("Enter the Author of book");
            Console.WriteLine("Select the Genre of the book");
            Genre GenreName = Genre.Fiction;
            int Genreselection = ValidateNumber("1. Fiction\n2. NonFiction\n3. Biography\n4. Children");
            switch (Genreselection)
            {
                case 1:
                    GenreName = Genre.Fiction;
                    break;
                case 2:
                    GenreName = Genre.NonFiction;
                    break;
                case 3:
                    GenreName = Genre.Biography;
                    break;
                case 4:
                    GenreName = Genre.Children;
                    break;
            }
            while (Genreselection > 4)
            {
                Console.WriteLine("\nYou have enterd wrong option. Please! select the right option\n");
                Genreselection = ValidateNumber("1. Fiction\n2. NonFiction\n3. Biography\n4. Children"); ;
                switch (Genreselection)
                {
                    case 1:
                        GenreName = Genre.Fiction;
                        break;
                    case 2:
                        GenreName = Genre.NonFiction;
                        break;
                    case 3:
                        GenreName = Genre.Biography;
                        break;
                    case 4:
                        GenreName = Genre.Children;
                        break;
                }
            }
            
            Console.WriteLine("Enter the publication date of the book");
            var day = ValidateNumber("DD : ");
            while (day <= 0 || day > 31)
            {
                Console.WriteLine("Day is not valid! Please enter a valid date");
                day = ValidateNumber("DD : ");
            }
            var month = ValidateNumber("MM : ");
            while (month <= 0 || month > 12)
            {
                Console.WriteLine("Month is not valid! Please enter a valid date");
                month = ValidateNumber("MM : ");
            }
            var year = ValidateNumber("YYYY : ");
            while (year >= DateTime.Now.Year)
            {
                Console.WriteLine("Year is not valid! Please enter a valid date");
                year = ValidateNumber("YYYY : ");
            }
            var publicationDate = new DateTime(year, month, day);
            Console.WriteLine("Enter the ISBN of the book");
            var isbn = Console.ReadLine();
            bool IsIsbn = ValidateIsbn(isbn);
            while (IsIsbn == false)
            {
                Console.WriteLine("Enter the ISBN in correct format");
                isbn = Console.ReadLine();
                IsIsbn = ValidateIsbn(isbn);
            }
            /*var isIsbnValid = Lms.CheckIsbn(isbn);
            while (isIsbnValid == false)
            {
                Console.WriteLine("This Isbn is already in use");
                isbn = Console.ReadLine();
                isIsbnValid = Lms.CheckIsbn(isbn);
            }*/
            var noOfPages = ValidateNumber("How many number of pages does the book contain");
            List<Book> UserBook = new List<Book>() { new Book() { Title = title, Author = author, GenreName = GenreName, PublicationDate = publicationDate, Isbn = isbn, NoOfPages = noOfPages}};
            return UserBook;
        }

        public static void ResponseRecived(Book seacrchedBook)
        {
            if (seacrchedBook != null)
            {
                DisplayDetails(seacrchedBook);
            }
            else
            {
                Console.WriteLine("Book not found");
            }
        }

        public static void DisplayDetails(Book book)
        {
            Console.WriteLine("Book Id : " + book.BookId + "\nTitle : " + book.Title + "\nAuthor : " + book.Author + "\nGenre : " + book.GenreName +
                "\nPublication Date : " + book.PublicationDate.ToShortDateString() + "\nISBN : " + book.Isbn + "\nNumber of Pages : " + book.NoOfPages);
            Console.WriteLine();
        }



        public static SecureString GetPassword()
        {
            var pwd = new SecureString();
            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd.RemoveAt(pwd.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else if (i.KeyChar != '\u0000') 
                {
                    pwd.AppendChar(i.KeyChar);
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return pwd;
        }


        public static void DisplayBooksByGenre(List<Book> SearchedBookByGenre)
        {
            foreach (var book in SearchedBookByGenre)
            {
                DisplayDetails(book);
            }
        }

        public static bool ContainSpecialChar(string userName)
        {
            string specialChar = @"\|!#$%&/()=?»«@£§€{}.-;'<>_,";
            foreach (var item in specialChar)
            {
                if (userName.Contains(item)) 
                    return true;
            }
            return false;
        }


        public static void EnterLibrary()
        {
            LibraryManagementSystem Library = new LibraryManagementSystem();
            int CardNo = 0;
            while (true)
            {
                Console.WriteLine("LIBRARY MANAGEMENT SYSTEM");
                int ChooseOption = ValidateNumber("1. User\n2. Librarian\n");
                
                if(ChooseOption == 2)
                {
                    var Logout = true;
                    while (Logout)
                    {
                        int UserInput = ValidateNumber("1. Add Book\n2. Search Book\n3. View All Books\n4. Logout");
                        switch (UserInput)
                        {
                            case 1:
                                var userBook = ReadInputFromUser();
                                int BookQuantity = ValidateNumber("Enter the book quantity");
                                Console.WriteLine();
                                for(int i =0; i< BookQuantity; i++)
                                {
                                    var bookId = Guid.NewGuid();
                                    Library.AddBook(bookId, userBook.FirstOrDefault().Title, userBook.FirstOrDefault().Author, userBook.FirstOrDefault().GenreName, userBook.FirstOrDefault().PublicationDate, userBook.FirstOrDefault().Isbn, userBook.FirstOrDefault().NoOfPages);
                                }
                                Console.WriteLine("The books have been added sucessfully\n");
                                break;
                            case 2:
                                var choice = ValidateNumber("1. Search by Book Id\n2. Search by title\n3. Search by Author\n4. Search by Genre");
                                switch (choice)
                                {
                                    case 1:
                                        Console.WriteLine("Enter the Book ID to search");
                                        var searchId = Console.ReadLine().ToLower();
                                        if(searchId.Length == 0)
                                        {
                                            Console.WriteLine("Book Not Found");
                                            break;
                                        }
                                        var searchedBookById = Library.SearchById(searchId);
                                        if (searchedBookById.Count == 0)
                                        {
                                            Console.WriteLine("Book Not Found");
                                            break;
                                        }
                                        foreach (var book in searchedBookById)
                                        {
                                            DisplayDetails(book);
                                        }
                                        break;
                                    case 2:
                                        Console.WriteLine("Enter the title of the book to search");
                                        var searchTitle = Console.ReadLine().ToLower();
                                        if (searchTitle.Length == 0)
                                        {
                                            Console.WriteLine("Book Not Found");
                                            break;
                                        }
                                        var doesSearchTitleContainSpecialChar = ContainSpecialChar(searchTitle);
                                        while (doesSearchTitleContainSpecialChar == true)
                                        {
                                            Console.WriteLine("\nCannot search using a special character");
                                            searchTitle = Console.ReadLine().ToLower();
                                            doesSearchTitleContainSpecialChar = ContainSpecialChar(searchTitle);
                                        }
                                        var searchedBookByTitle = Library.SearchByTitle(searchTitle);
                                        if (searchedBookByTitle.Count == 0)
                                        {
                                            Console.WriteLine("Book Not Found");
                                            break;
                                        }
                                        foreach (var book in searchedBookByTitle)
                                        {
                                            DisplayDetails(book);
                                        }
                                        break;
                                    case 3:
                                        Console.WriteLine("Enter the Author of the book to search");
                                        var searchAuthor = Console.ReadLine().ToLower();
                                        if (searchAuthor.Length == 0)
                                        {
                                            Console.WriteLine("Book Not Found");
                                            break;
                                        }
                                        var doesSearchAuthorContainSpecialChar = ContainSpecialChar(searchAuthor);
                                        while (doesSearchAuthorContainSpecialChar == true)
                                        {
                                            Console.WriteLine("\nCannot search using a special character");
                                            searchAuthor = Console.ReadLine().ToLower();
                                            doesSearchAuthorContainSpecialChar = ContainSpecialChar(searchAuthor);
                                        }
                                        var searchedBookByAuthor = Library.SearchByAuthor(searchAuthor);
                                        if (searchedBookByAuthor.Count == 0)
                                        {
                                            Console.WriteLine("Book Not Found");
                                        }
                                        foreach (var book in searchedBookByAuthor)
                                        {
                                            DisplayDetails(book);
                                        }
                                        break;
                                    case 4:
                                        var searchGenre = ValidateNumber("Select the Genre of the book to search\n1. Fiction\n2. NonFiction\n3. Biography\n4. Children");
                                        switch (searchGenre)
                                        {
                                            case 1:
                                                List<Book> SearchedBookByGenre = Library.SearchByGenre(Genre.Fiction);
                                                DisplayBooksByGenre(SearchedBookByGenre);
                                                break;
                                            case 2:
                                                SearchedBookByGenre = Library.SearchByGenre(Genre.NonFiction);
                                                DisplayBooksByGenre(SearchedBookByGenre);
                                                break;
                                            case 3:
                                                SearchedBookByGenre = Library.SearchByGenre(Genre.Biography);
                                                DisplayBooksByGenre(SearchedBookByGenre);
                                                break;
                                            case 4:
                                                SearchedBookByGenre = Library.SearchByGenre(Genre.Children);
                                                DisplayBooksByGenre(SearchedBookByGenre);
                                                break;
                                            default:
                                                Console.WriteLine("Wrong Option");
                                                break;
                                        }
                                        break;
                                }
                                while (choice > 4)
                                {
                                    Console.WriteLine("\nWrong Option! Select from the options listed above\n");
                                    choice = ValidateNumber("1. Search by Book Id\n2. Search by title\n3. Search by Author\n4. Search by Genre");
                                    switch (choice)
                                    {
                                        case 1:
                                            Console.WriteLine("Enter the Book ID to search");
                                            var searchId = Console.ReadLine().ToLower();
                                            if (searchId.Length == 0)
                                            {
                                                Console.WriteLine("Book Not Found");
                                                break;
                                            }
                                            var searchedBookById = Library.SearchById(searchId);
                                            if (searchedBookById.Count == 0)
                                            {
                                                Console.WriteLine("Book Not Found");
                                                break;
                                            }
                                            foreach (var book in searchedBookById)
                                            {
                                                DisplayDetails(book);
                                            }
                                            break;
                                        case 2:
                                            Console.WriteLine("Enter the title of the book to search");
                                            var searchTitle = Console.ReadLine().ToLower();
                                            if (searchTitle.Length == 0)
                                            {
                                                Console.WriteLine("Book Not Found");
                                                break;
                                            }
                                            var doesSearchTitleContainSpecialChar = ContainSpecialChar(searchTitle);
                                            while (doesSearchTitleContainSpecialChar == true)
                                            {
                                                Console.WriteLine("\nCannot search using a special character");
                                                searchTitle = Console.ReadLine().ToLower();
                                                doesSearchTitleContainSpecialChar = ContainSpecialChar(searchTitle);
                                            }
                                            var searchedBookByTitle = Library.SearchByTitle(searchTitle);
                                            if (searchedBookByTitle.Count == 0)
                                            {
                                                Console.WriteLine("Book Not Found");
                                                break;
                                            }
                                            foreach (var book in searchedBookByTitle)
                                            {
                                                DisplayDetails(book);
                                            }
                                            break;
                                        case 3:
                                            Console.WriteLine("Enter the Author of the book to search");
                                            var searchAuthor = Console.ReadLine().ToLower();
                                            if (searchAuthor.Length == 0)
                                            {
                                                Console.WriteLine("Book Not Found");
                                                break;
                                            }
                                            var doesSearchAuthorContainSpecialChar = ContainSpecialChar(searchAuthor);
                                            while (doesSearchAuthorContainSpecialChar == true)
                                            {
                                                Console.WriteLine("\nCannot search using a special character");
                                                searchAuthor = Console.ReadLine().ToLower();
                                                doesSearchAuthorContainSpecialChar = ContainSpecialChar(searchAuthor);
                                            }
                                            var searchedBookByAuthor = Library.SearchByAuthor(searchAuthor);
                                            if (searchedBookByAuthor.Count == 0)
                                            {
                                                Console.WriteLine("Book Not Found");
                                            }
                                            foreach (var book in searchedBookByAuthor)
                                            {
                                                DisplayDetails(book);
                                            }
                                            break;
                                        case 4:
                                            var searchGenre = ValidateNumber("Select the Genre of the book to search\n1. Fiction\n2. NonFiction\n3. Biography\n4. Children");
                                            switch (searchGenre)
                                            {
                                                case 1:
                                                    List<Book> SearchedBookByGenre = Library.SearchByGenre(Genre.Fiction);
                                                    DisplayBooksByGenre(SearchedBookByGenre);
                                                    break;
                                                case 2:
                                                    SearchedBookByGenre = Library.SearchByGenre(Genre.NonFiction);
                                                    DisplayBooksByGenre(SearchedBookByGenre);
                                                    break;
                                                case 3:
                                                    SearchedBookByGenre = Library.SearchByGenre(Genre.Biography);
                                                    DisplayBooksByGenre(SearchedBookByGenre);
                                                    break;
                                                case 4:
                                                    SearchedBookByGenre = Library.SearchByGenre(Genre.Children);
                                                    DisplayBooksByGenre(SearchedBookByGenre);
                                                    break;
                                                default:
                                                    Console.WriteLine("Wrong Option");
                                                    break;
                                            }
                                            break;
                                    }

                                }
                                break;
                            case 3:
                                
                                if (Library.Books.Count > 0)
                                    Library.ViewAllBooks();
                                else
                                    Console.WriteLine("\nThere are no books in library to display\n");
                                break;
                            case 4:
                                Logout = false;
                                Console.WriteLine("\nYou have been logged out!\n");
                                break;
                            default:
                                Console.WriteLine("\nWrong Option\n");
                                break;
                        }
                    }   
                }

                else if(ChooseOption == 1)
                {
                    int UserChoice = ValidateNumber("\n1. New User? Register \n2. Registed User\n");
                    if (UserChoice == 1)
                    {
                        CardNo++;
                        var userId = Guid.NewGuid();
                        Console.Write("User Name : ");
                        var userName = Console.ReadLine();
                        while (userName.Length == 0)
                        {
                            Console.WriteLine("\nUser Name cannot be null\nUser Name : ");
                            userName = Console.ReadLine();
                        }
                        var doesUserNameContainSpecialChar = ContainSpecialChar(userName);
                        while(doesUserNameContainSpecialChar == true)
                        {
                            Console.WriteLine("\nSorry! User name cannot contain special characters\nUser Name : ");
                            userName = Console.ReadLine();
                            doesUserNameContainSpecialChar = ContainSpecialChar(userName);
                        }
                        var IsUserNameUnique = CheckUserName(userName);
                        while (IsUserNameUnique == true)
                        {
                            Console.WriteLine("\nSorry! the user name is already in usage. Please, Enter a different user name\nUser Name : ");
                            userName = Console.ReadLine();
                            IsUserNameUnique = CheckUserName(userName);
                        }
                        Console.WriteLine("\nPassword : ");
                        var password = GetPassword();
                        while (password.Length == 0)
                        {
                            Console.WriteLine("\nPassword cannot be null\nPassword : ");
                            password = GetPassword();
                        }
                        var ConvertedPassword = new NetworkCredential("", password).Password;
                        LibraryCards.Add(new LibraryCard(CardNo, userId, userName, ConvertedPassword));
                        Console.WriteLine("\nCongratulations! You have been registered. Kindly Go back and Login\n");
                    }

                    else if (UserChoice == 2)
                    {
                        Console.Write("User Name : ");
                        var userName = Console.ReadLine();
                        while (userName.Length == 0)
                        {
                            Console.WriteLine("\nUser Name cannot be null\nUser Name : ");
                            userName = Console.ReadLine();
                        }
                        Console.WriteLine("Password : ");
                        var password = GetPassword();
                        var ConvertedPassword = new NetworkCredential("", password).Password;
                        while (ConvertedPassword.Length == 0)
                        {
                            Console.WriteLine("\nPassword cannot be null\nPassword : ");
                            password = GetPassword();
                            ConvertedPassword = new NetworkCredential("", password).Password;
                        }
                        bool IsUserValid = ValidateUser(userName, ConvertedPassword);
                        if (IsUserValid)
                        {
                            var UserCard = LibraryCards.Find(card => card.UserName == userName);
                            var Logout = true;
                            Console.WriteLine("\nCongratulations! You have been logged in succesfully\n");
                            while (Logout)
                            {
                                int UserInput = ValidateNumber("1. Search book\n2. Checkout book\n3. view all checkout books\n4. Return a book\n5. Library Card Details\n6. Logout");
                                switch (UserInput)
                                {
                                    case 1:
                                        var choice = ValidateNumber("1. Search by Book Id\n2. Search by title\n3. Search by Author\n4. Search by Genre");
                                        switch (choice)
                                        {
                                            case 1:
                                                Console.WriteLine("Enter the Book ID to search");
                                                var searchId = Console.ReadLine().ToLower();
                                                if (searchId.Length == 0)
                                                {
                                                    Console.WriteLine("Book Not Found");
                                                    break;
                                                }
                                                var searchedBookById = Library.SearchById(searchId);
                                                if (searchedBookById.Count == 0)
                                                {
                                                    Console.WriteLine("Book Not Found");
                                                    break;
                                                }
                                                foreach (var book in searchedBookById)
                                                {
                                                    DisplayDetails(book);
                                                }
                                                break;
                                            case 2:
                                                Console.WriteLine("Enter the title of the book to search");
                                                var searchTitle = Console.ReadLine().ToLower();
                                                if (searchTitle.Length == 0)
                                                {
                                                    Console.WriteLine("Book Not Found");
                                                    break;
                                                }
                                                var doesSearchTitleContainSpecialChar = ContainSpecialChar(searchTitle);
                                                while (doesSearchTitleContainSpecialChar == true)
                                                {
                                                    Console.WriteLine("\nCannot search using a special character");
                                                    searchTitle = Console.ReadLine().ToLower();
                                                    doesSearchTitleContainSpecialChar = ContainSpecialChar(searchTitle);
                                                }
                                                var searchedBookByTitle = Library.SearchByTitle(searchTitle);
                                                if (searchedBookByTitle.Count == 0)
                                                {
                                                    Console.WriteLine("Book Not Found");
                                                    break;
                                                }
                                                foreach (var book in searchedBookByTitle)
                                                {
                                                    DisplayDetails(book);
                                                }
                                                break;
                                            case 3:
                                                Console.WriteLine("Enter the Author of the book to search");
                                                var searchAuthor = Console.ReadLine().ToLower();
                                                if (searchAuthor.Length == 0)
                                                {
                                                    Console.WriteLine("Book Not Found");
                                                    break;
                                                }
                                                var doesSearchAuthorContainSpecialChar = ContainSpecialChar(searchAuthor);
                                                while (doesSearchAuthorContainSpecialChar == true)
                                                {
                                                    Console.WriteLine("\nCannot search using a special character");
                                                    searchAuthor = Console.ReadLine().ToLower();
                                                    doesSearchAuthorContainSpecialChar = ContainSpecialChar(searchAuthor);
                                                }
                                                var searchedBookByAuthor = Library.SearchByAuthor(searchAuthor);
                                                if (searchedBookByAuthor.Count == 0)
                                                {
                                                    Console.WriteLine("Book Not Found");
                                                }
                                                foreach (var book in searchedBookByAuthor)
                                                {
                                                    DisplayDetails(book);
                                                }
                                                break;
                                            case 4:
                                                var searchGenre = ValidateNumber("Select the Genre of the book to search\n1. Fiction\n2. NonFiction\n3. Biography\n4. Children");
                                                switch (searchGenre)
                                                {
                                                    case 1:
                                                        List<Book> SearchedBookByGenre = Library.SearchByGenre(Genre.Fiction);
                                                        DisplayBooksByGenre(SearchedBookByGenre);
                                                        break;
                                                    case 2:
                                                        SearchedBookByGenre = Library.SearchByGenre(Genre.NonFiction);
                                                        DisplayBooksByGenre(SearchedBookByGenre);
                                                        break;
                                                    case 3:
                                                        SearchedBookByGenre = Library.SearchByGenre(Genre.Biography);
                                                        DisplayBooksByGenre(SearchedBookByGenre);
                                                        break;
                                                    case 4:
                                                        SearchedBookByGenre = Library.SearchByGenre(Genre.Children);
                                                        DisplayBooksByGenre(SearchedBookByGenre);
                                                        break;
                                                    default:
                                                        Console.WriteLine("Wrong Option");
                                                        break;
                                                }
                                                break;
                                        }

                                        while (choice > 4)
                                        {
                                            Console.WriteLine("\nWrong Option! Select from the options listed above\n");
                                            choice = ValidateNumber("1. Search by Book Id\n2. Search by title\n3. Search by Author\n4. Search by Genre");
                                            switch (choice)
                                            {
                                                case 1:
                                                    Console.WriteLine("Enter the Book ID to search");
                                                    var searchId = Console.ReadLine().ToLower();
                                                    if (searchId.Length == 0)
                                                    {
                                                        Console.WriteLine("Book Not Found");
                                                        break;
                                                    }
                                                    var searchedBookById = Library.SearchById(searchId);
                                                    if (searchedBookById.Count == 0)
                                                    {
                                                        Console.WriteLine("Book Not Found");
                                                        break;
                                                    }
                                                    foreach (var book in searchedBookById)
                                                    {
                                                        DisplayDetails(book);
                                                    }
                                                    break;
                                                case 2:
                                                    Console.WriteLine("Enter the title of the book to search");
                                                    var searchTitle = Console.ReadLine().ToLower();
                                                    if (searchTitle.Length == 0)
                                                    {
                                                        Console.WriteLine("Book Not Found");
                                                        break;
                                                    }
                                                    var doesSearchTitleContainSpecialChar = ContainSpecialChar(searchTitle);
                                                    while (doesSearchTitleContainSpecialChar == true)
                                                    {
                                                        Console.WriteLine("\nCannot search using a special character");
                                                        searchTitle = Console.ReadLine().ToLower();
                                                        doesSearchTitleContainSpecialChar = ContainSpecialChar(searchTitle);
                                                    }
                                                    var searchedBookByTitle = Library.SearchByTitle(searchTitle);
                                                    if (searchedBookByTitle.Count == 0)
                                                    {
                                                        Console.WriteLine("Book Not Found");
                                                        break;
                                                    }
                                                    foreach (var book in searchedBookByTitle)
                                                    {
                                                        DisplayDetails(book);
                                                    }
                                                    break;
                                                case 3:
                                                    Console.WriteLine("Enter the Author of the book to search");
                                                    var searchAuthor = Console.ReadLine().ToLower();
                                                    if (searchAuthor.Length == 0)
                                                    {
                                                        Console.WriteLine("Book Not Found");
                                                        break;
                                                    }
                                                    var doesSearchAuthorContainSpecialChar = ContainSpecialChar(searchAuthor);
                                                    while (doesSearchAuthorContainSpecialChar == true)
                                                    {
                                                        Console.WriteLine("\nCannot search using a special character");
                                                        searchAuthor = Console.ReadLine().ToLower();
                                                        doesSearchAuthorContainSpecialChar = ContainSpecialChar(searchAuthor);
                                                    }
                                                    var searchedBookByAuthor = Library.SearchByAuthor(searchAuthor);
                                                    if (searchedBookByAuthor.Count == 0)
                                                    {
                                                        Console.WriteLine("Book Not Found");
                                                    }
                                                    foreach (var book in searchedBookByAuthor)
                                                    {
                                                        DisplayDetails(book);
                                                    }
                                                    break;
                                                case 4:
                                                    var searchGenre = ValidateNumber("Select the Genre of the book to search\n1. Fiction\n2. NonFiction\n3. Biography\n4. Children");
                                                    switch (searchGenre)
                                                    {
                                                        case 1:
                                                            List<Book> SearchedBookByGenre = Library.SearchByGenre(Genre.Fiction);
                                                            DisplayBooksByGenre(SearchedBookByGenre);
                                                            break;
                                                        case 2:
                                                            SearchedBookByGenre = Library.SearchByGenre(Genre.NonFiction);
                                                            DisplayBooksByGenre(SearchedBookByGenre);
                                                            break;
                                                        case 3:
                                                            SearchedBookByGenre = Library.SearchByGenre(Genre.Biography);
                                                            DisplayBooksByGenre(SearchedBookByGenre);
                                                            break;
                                                        case 4:
                                                            SearchedBookByGenre = Library.SearchByGenre(Genre.Children);
                                                            DisplayBooksByGenre(SearchedBookByGenre);
                                                            break;
                                                        default:
                                                            Console.WriteLine("Wrong Option");
                                                            break;
                                                    }
                                                    break;
                                            }

                                        }
                                        break;
                                    case 2:
                                        Console.WriteLine("Enter the Book Id to checkout");
                                        var checkoutBookId = Console.ReadLine();
                                        var checkoutBook = Library.Books.Where(book => book.BookId.ToString() == checkoutBookId).FirstOrDefault();
                                        if (checkoutBook == null)
                                        {
                                            Console.WriteLine("Book not found to checkout");
                                            break;
                                        }
                                        checkoutBook.BorrowedTime = DateTime.Now;
                                        UserCard.BooksBorrowed.Add(checkoutBook);
                                        Library.CheckoutBook(checkoutBook);
                                        Console.WriteLine("The book has been checked out for you\n");
                                        break;
                                    case 3:
                                        var allCheckedBooks = UserCard.BooksBorrowed;
                                        if (allCheckedBooks == null)
                                        {
                                            Console.WriteLine("There are no books in your checkout list");
                                            break;
                                        }
                                        Console.WriteLine($"The number of books you have checked out are " + allCheckedBooks.Count);
                                        foreach (var book in allCheckedBooks)
                                        {
                                            Console.WriteLine("Book Id : " + book.BookId + "\nTitle : " + book.Title + "\nAuthor : " + book.Author + "\nGenre : " + book.GenreName + 
                                                "\nPublication Date : " + book.PublicationDate.ToShortDateString() + "\nISBN : " + book.Isbn + "\nNumber of Pages : " + book.NoOfPages +
                                                "\nBorrowed Time : " + book.BorrowedTime);
                                            Console.WriteLine();
                                        }
                                        break;
                                    case 4:
                                        var ReturnNoOfBooks = ValidateNumber("How many books do you want to return");
                                        if (ReturnNoOfBooks > UserCard.BooksBorrowed.Count)
                                        {
                                            Console.WriteLine("You do not have that many books to return");
                                            break;
                                        }
                                        for (int i = 0; i < ReturnNoOfBooks; i++)
                                        {
                                            Console.WriteLine("Enter the Book Id to return");
                                            var ReturnBookId = Console.ReadLine();
                                            var retrunBook = UserCard.BooksBorrowed.Where(book => book.BookId.ToString() == ReturnBookId).FirstOrDefault();
                                            if (retrunBook == null)
                                            {
                                                Console.WriteLine("Your checkout list does not contain that book");
                                                break;
                                            }
                                            else
                                            {
                                                var ReturnBook = UserCard.BooksBorrowed.Find(book => book.BookId.ToString() == ReturnBookId);
                                                ReturnBook.ReturnTime = DateTime.Now;
                                                UserCard.BooksBorrowed.Remove(ReturnBook);
                                                UserCard.BooksReturned.Add(ReturnBook);
                                                Library.ReturnBook(ReturnBook);
                                                Console.WriteLine("The book returned succesfully");
                                                break;
                                            }
                                        }
                                        break;
                                    case 5:
                                        Console.WriteLine("Card Details\n");
                                        Console.WriteLine("Card No : " + UserCard.LibraryCardNo + "\nUser Id : " + UserCard.UserId + "\nUser Name : " + UserCard.UserName);
                                        Console.WriteLine("\nTransaction History : ");
                                        var allBooksReturned = UserCard.BooksReturned;
                                        if (allBooksReturned == null)
                                        {
                                            Console.WriteLine("There are no transaction history");
                                            break;
                                        }
                                        foreach (var book in allBooksReturned)
                                        {
                                            Console.WriteLine("Title : " + book.Title + "\nAuthor: " + book.Author + "\nGenre: " + book.GenreName + "\nPublication Date : " +
                                                book.PublicationDate.ToShortDateString() + "\nISBN : " + book.Isbn + "\nNumber of Pages : " + book.NoOfPages + "\nBorrowed Time : " + 
                                                book.BorrowedTime + "\nReturn Time : " + book.ReturnTime + "\n");
                                        }
                                        break;
                                    case 6:
                                        Logout = false;
                                        Console.WriteLine("\nYou have been logged out Sucessfully!\n");
                                        break;
                                    default:
                                        Console.WriteLine("\nWrong Option! Select from the options listed\n");
                                        break;
                                }
                            }

                        }
                        else
                        {
                            Console.WriteLine("\nWrong User Name or Password\n");
                        }

                    }

                    while (UserChoice > 2)
                    {
                        Console.WriteLine("\nWrong Option! Select from the option above\n");
                        UserChoice = ValidateNumber("\n1. New User? Register \n2. Registed User\n");
                        if (UserChoice == 1)
                        {
                            CardNo++;
                            var userId = Guid.NewGuid();
                            Console.Write("User Name : ");
                            var userName = Console.ReadLine();
                            while (userName == null)
                            {
                                Console.WriteLine("User Name cannot be null");
                                userName = Console.ReadLine();
                            }
                            var IsUserNameUnique = CheckUserName(userName);
                            while (IsUserNameUnique == true)
                            {
                                Console.WriteLine("Sorry! the user name is already in usage. Please, Enter a different user name");
                                userName = Console.ReadLine();
                                IsUserNameUnique = CheckUserName(userName);
                            }
                            Console.WriteLine("Password : ");
                            var password = GetPassword();
                            while (password == null)
                            {
                                Console.WriteLine("Password cannot be null");
                                password = GetPassword();
                            }
                            var ConvertedPassword = new NetworkCredential("", password).Password;
                            LibraryCards.Add(new LibraryCard(CardNo, userId, userName, ConvertedPassword));
                            Console.WriteLine("Congratulations! You have been registered. Kindly Go back and Login\n");
                        }

                        else if (UserChoice == 2)
                        {
                            Console.Write("User Name : ");
                            var userName = Console.ReadLine();
                            Console.WriteLine("Password : ");
                            var password = GetPassword();
                            var ConvertedPassword = new NetworkCredential("", password).Password;
                            bool IsUserValid = ValidateUser(userName, ConvertedPassword);
                            if (IsUserValid)
                            {
                                var UserCard = LibraryCards.Find(card => card.UserName == userName);
                                var Logout = true;
                                Console.WriteLine("\nCongratulations! You have been logged in succesfully\n");
                                while (Logout)
                                {
                                    int UserInput = ValidateNumber("1. Search book\n2. Checkout book\n3. view all checkout books\n4. Return a book\n5. Library Card Details\n6. Logout");
                                    switch (UserInput)
                                    {
                                        case 1:
                                            var choice = ValidateNumber("1. Search by Book Id\n2. Search by title\n3. Search by Author\n4. Search by Genre");
                                            switch (choice)
                                            {
                                                case 1:
                                                    Console.WriteLine("Enter the Book ID to search");
                                                    var searchId = Console.ReadLine().ToLower();
                                                    var searchedBookById = Library.SearchById(searchId);
                                                    foreach (var book in searchedBookById)
                                                    {
                                                        DisplayDetails(book);
                                                    }
                                                    break;
                                                case 2:
                                                    Console.WriteLine("Enter the title of the book to search");
                                                    var searchTitle = Console.ReadLine().ToLower();
                                                    var searchedBookByTitle = Library.SearchByTitle(searchTitle);
                                                    foreach (var book in searchedBookByTitle)
                                                    {
                                                        DisplayDetails(book);
                                                    }
                                                    break;
                                                case 3:
                                                    Console.WriteLine("Enter the Author of the book to search");
                                                    var searchAuthor = Console.ReadLine().ToLower();
                                                    var searchedBookByAuthor = Library.SearchByAuthor(searchAuthor);
                                                    foreach (var book in searchedBookByAuthor)
                                                    {
                                                        DisplayDetails(book);
                                                    }
                                                    break;
                                                case 4:
                                                    var searchGenre = ValidateNumber("Select the Genre of the book to search\n1. Fiction\n2. NonFiction\n3. Biography\n4. Children");
                                                    switch (searchGenre)
                                                    {
                                                        case 1:
                                                            List<Book> SearchedBookByGenre = Library.SearchByGenre(Genre.Fiction);
                                                            DisplayBooksByGenre(SearchedBookByGenre);
                                                            break;
                                                        case 2:
                                                            SearchedBookByGenre = Library.SearchByGenre(Genre.NonFiction);
                                                            DisplayBooksByGenre(SearchedBookByGenre);
                                                            break;
                                                        case 3:
                                                            SearchedBookByGenre = Library.SearchByGenre(Genre.Biography);
                                                            DisplayBooksByGenre(SearchedBookByGenre);
                                                            break;
                                                        case 4:
                                                            SearchedBookByGenre = Library.SearchByGenre(Genre.Children);
                                                            DisplayBooksByGenre(SearchedBookByGenre);
                                                            break;
                                                        default:
                                                            Console.WriteLine("Wrong Option");
                                                            break;
                                                    }
                                                    break;
                                                default:
                                                    Console.WriteLine("Wrong Option");
                                                    break;
                                            }
                                            break;
                                        case 2:
                                            Console.WriteLine("Enter the Book Id to checkout");
                                            var checkoutBookId = Console.ReadLine();
                                            var checkoutBook = Library.Books.Where(book => book.BookId.ToString() == checkoutBookId).FirstOrDefault();
                                            if (checkoutBook == null)
                                            {
                                                Console.WriteLine("Book not found to checkout");
                                                break;
                                            }
                                            UserCard.BooksBorrowed.Add(checkoutBook);
                                            Library.CheckoutBook(checkoutBook);
                                            Console.WriteLine("The book has been checked out for you\n");
                                            break;
                                        case 3:
                                            var allCheckedBooks = UserCard.BooksBorrowed;
                                            if (allCheckedBooks == null)
                                            {
                                                Console.WriteLine("There are no books in your checkout list");
                                                break;
                                            }
                                            Console.WriteLine($"The number of books you have checked out are " + allCheckedBooks.Count);
                                            foreach (var book in allCheckedBooks)
                                            {
                                                Console.WriteLine("Title : " + book.Title);
                                            }
                                            break;
                                        case 4:
                                            var ReturnNoOfBooks = ValidateNumber("How many books do you want to return");
                                            if (ReturnNoOfBooks > UserCard.BooksBorrowed.Count)
                                            {
                                                Console.WriteLine("You do not have that many books to return");
                                                break;
                                            }
                                            for (int i = 0; i < ReturnNoOfBooks; i++)
                                            {
                                                Console.WriteLine("Enter the Book Id to return");
                                                var ReturnBookId = Console.ReadLine();



                                                var retrunBook = UserCard.BooksBorrowed.Where(book => book.BookId.ToString() == ReturnBookId).FirstOrDefault();
                                                if (retrunBook == null)
                                                {
                                                    Console.WriteLine("Your checkout list does not contain that book");
                                                    break;
                                                }
                                                else
                                                {
                                                    var ReturnBook = UserCard.BooksBorrowed.Find(book => book.BookId.ToString() == ReturnBookId);
                                                    Library.ReturnBook(ReturnBook);
                                                    Console.WriteLine("The book returned succesfully");
                                                    break;
                                                }
                                            }
                                            break;
                                        case 5:
                                            Console.WriteLine("Card Details");
                                            Console.WriteLine("Card No : " + UserCard.LibraryCardNo + "\nUser Id : " + UserCard.UserId + "\nUser Name : " + UserCard.UserName);
                                            break;
                                        case 6:
                                            Logout = false;
                                            Console.WriteLine("\nYou have been logged out Sucessfully!\n");
                                            break;
                                    }
                                }

                            }
                            else
                            {
                                Console.WriteLine("\nWrong User Name or Password\n");
                            }

                        }
                    }  
                }
                else
                {
                    Console.WriteLine("\nEnter a Valid option\n");
                }
                
            }
        }
        #endregion

        static void Main(string[] args)
        {
            EnterLibrary();
        }
    }
}
