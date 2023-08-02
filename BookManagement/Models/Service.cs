using BookManagement.Models;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace BookManagement.Models
{
    public class Service
    {
        private readonly string _dataFile = @"Data\data.xml";
        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(HashSet<Book>));
        public HashSet<Book> Books { get; set; }
        public Service()
        {
            if (!File.Exists(_dataFile))
            {
                Books = new HashSet<Book>() {
                    new Book{Id = 1, Name = "ASP.NET Core", Authors = "Adam Freeman", Publisher = "Washington", Year = 2022},
                    new Book{Id = 2, Name = "Pro ASP.NET Core", Authors = "Adam Freeman", Publisher = "NewYork", Year = 2023},
                    new Book{Id = 3, Name = "ASP.NET Core Video course", Authors = "Mark J. Price", Publisher = "California", Year = 2020},
                    new Book{Id = 4, Name = "War and Peace", Authors = "Lev Tolstoy", Publisher = "Washington", Year = 2015},
                    new Book{Id = 5, Name = "C# Programming", Authors = "Griffiths", Publisher = "O'Reilly Media", Year = 2020},
                };
            }
            else
            {
                using var stream = File.OpenRead(_dataFile);
                Books = _serializer.Deserialize(stream) as HashSet<Book>;
            }
        }
        public Book[] Get() => Books.ToArray();
        public Book Get(int id) => Books.FirstOrDefault(b => b.Id == id);
        public bool Add(Book book)
        {
            var max = Books.Max(b => b.Id);
            max = max + 1;
            book.Id = max;
            return Books.Add(book);
        }
        public int CountBook()
        {
            return Books.Count();
        }
        public Book Create()
        {
            var max = Books.Max(b => b.Id);
            var b = new Book()
            {
                Id = max + 1,
                Year = DateTime.Now.Year
            };
            return b;
        }
        public bool Update(Book book)
        {
            var b = Get(book.Id);
            return b != null && Books.Remove(b) && Books.Add(book);
        }
        public bool Delete(int id)
        {
            var b = Get(id);
            return b != null && Books.Remove(b);
        }
        public void SaveChanges()
        {
            using var stream = File.Create(_dataFile);
            _serializer.Serialize(stream, Books);
        }
        //upload file
        public string GetDataPath(string file) => $"Data\\{file}";
        public void Upload(Book book, IFormFile file)
        {
            if (file != null)
            {
                var path = GetDataPath(file.FileName);
                using var stream = new FileStream(path, FileMode.Create);
                file.CopyTo(stream);
                book.DataFile = file.FileName;
            }
        }
        //download file
        public (Stream, string) Download(Book b)
        {
            var memory = new MemoryStream();
            using var stream = new FileStream(GetDataPath(b.DataFile), FileMode.Open);
            stream.CopyTo(memory);
            memory.Position = 0;
            var type = Path.GetExtension(b.DataFile) switch
            {
                "pdf" => "application/pdf",
                "docx" => "application/vnd.ms-word",
                "doc" => "application/vnd.ms-word",
                "txt" => "text/plain",
                _ => "application/pdf"
            };
            return (memory, type);
        }

        //Paging
        public (Book[] books, int pages, int page) Paging(int page)
        {
            int size = 5;
            int pages = (int)Math.Ceiling((double)Books.Count / size);
            var books = Books.Skip((page - 1) * size).Take(size).ToArray();
            return (books, pages, page);
        }
        //Search
        public Book[] Get(string search)
        {
            var s = search.ToLower();
            return Books.Where(b =>
                b.Name.ToLower().Contains(s) ||
                b.Authors.ToLower().Contains(s) ||
                b.Publisher.ToLower().Contains(s) ||
                b.Year.ToString() == s
            ).ToArray();
        }

    }
}


