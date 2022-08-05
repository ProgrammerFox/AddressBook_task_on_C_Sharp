Console.WriteLine(@"Address Book by
 ____                                                                       _____              
|  _ \  _ __   ___    __ _  _ __   __ _  _ __ ___   _ __ ___    ___  _ __  |  ___|  ___  __  __
| |_) || '__| / _ \  / _` || '__| / _` || '_ ` _ \ | '_ ` _ \  / _ \| '__| | |_    / _ \ \ \/ /
|  __/ | |   | (_) || (_| || |   | (_| || | | | | || | | | | ||  __/| |    |  _|  | (_) | >  < 
|_|    |_|    \___/  \__, ||_|    \__,_||_| |_| |_||_| |_| |_| \___||_|    |_|     \___/ /_/\_\
                     |___/                                                                     ");
Console.WriteLine("Commands: \"add\", \"remove\", \"change\", \"swap\", \"show\", \"clear\", \"sort\", \"save\", \"open\", \"help\", \"stop/exit\"\n");

ICommunicator communicator = new ConsoleCommunicator();

AddressBook addressBook = new AddressBook(communicator, new ConsoleDisplayer(communicator), new BookFileManagerAStxt(communicator));

ICommander commander = new TXTCommander(communicator);

while (true)
{
    int result = commander.Execute(addressBook);
    if (result == 1) break;
    else if (result == 2) continue;
}


interface ICommunicator
{
    void Print(String Message);
    String? Input();
}

class ConsoleCommunicator : ICommunicator
{
    public void Print(String Message)
    {
        Console.Write(Message);
    }
    public String? Input()
    {
        return Console.ReadLine();
    }
}

interface ICommander
{
    int Execute(AddressBook addressBook);
}

class TXTCommander : ICommander
{
    private ICommunicator Communicator;
    public TXTCommander(ICommunicator communicator)
    {
        Communicator = communicator;
    }

    private bool SortKey(String type, out AddressBook.SortBy sortBy)
    {
        sortBy = AddressBook.SortBy.FirstName;
        switch (type)
        {
            case "firstname":
                sortBy = AddressBook.SortBy.FirstName;
                break;
            case "lastname":
                sortBy = AddressBook.SortBy.LastName;
                break;
            case "address":
                sortBy = AddressBook.SortBy.Address;
                break;
            default:
                return false;
        }

        return true;
    }

    public int Execute(AddressBook addressBook)
    {
        Communicator.Print("Command: ");
        String? line = Communicator.Input();
        if (line == null) return 2;
        String[] command = line.Split();
        switch (command[0])
        {
            case "add":
                if (command.Length == 4) addressBook.Add(new PersonInfo(command[1], command[2], command[3]));
                else Communicator.Print("Error!\n");
                break;
            case "change":
                {
                    int num;
                    if (command.Length == 5 && int.TryParse(command[1], out num)) addressBook.Change(num - 1, new PersonInfo(command[2], command[3], command[4]));
                    else Communicator.Print("Error!\n");
                    break;
                }
            case "swap":
                {
                    int index1, index2;
                    if (command.Length == 3 && int.TryParse(command[1], out index1) && int.TryParse(command[2], out index2)) addressBook.Swap(index1 - 1, index2 - 1);
                    else Communicator.Print("Error!\n");
                    break;
                }
            case "show":
                addressBook.Show();
                break;
            case "remove":
                {
                    int num;
                    if (command.Length == 2 && int.TryParse(command[1], out num)) addressBook.Remove(num - 1);
                    else Communicator.Print("Error!\n");
                    break;
                }
            case "clear":
                addressBook.Clear();
                break;
            case "sort":
                bool sortType = false;
                if (command.Length > 1 && command[command.Length - 1].ToLower() == "desc")
                {
                    sortType = true;
                    command = command.Take(command.Count() - 1).ToArray();
                }

                if (command.Length == 1) addressBook.Sort(sortType);
                else if (command.Length == 2)
                {
                    AddressBook.SortBy sortBy;
                    if (SortKey(command[1].ToLower(), out sortBy)) addressBook.Sort(sortBy, sortType);
                    else Communicator.Print("Error!\n");
                }
                else if (command.Length == 3)
                {
                    AddressBook.SortBy sortBy1;
                    AddressBook.SortBy sortBy2;
                    if (SortKey(command[1].ToLower(), out sortBy1) && SortKey(command[2].ToLower(), out sortBy2)) addressBook.Sort(sortBy1, sortBy2, sortType);
                    else Communicator.Print("Error!\n");

                }
                else if (command.Length == 4)
                {
                    AddressBook.SortBy sortBy1;
                    AddressBook.SortBy sortBy2;
                    AddressBook.SortBy sortBy3;
                    if (SortKey(command[1].ToLower(), out sortBy1) && SortKey(command[2].ToLower(), out sortBy2) && SortKey(command[3].ToLower(), out sortBy3)) addressBook.Sort(sortBy1, sortBy2, sortBy3, sortType);
                    else Communicator.Print("Error!\n");

                }
                else Communicator.Print("Error!\n");
                break;
            case "stop":
                return 1;
            case "exit":
                return 1;
            case "save":
                if (command.Length == 3) addressBook.Save(command[1], command[2]);
                else Communicator.Print("Error!\n");
                break;
            case "open":
                if (command.Length == 3) addressBook.Open(command[1], command[2]);
                else Communicator.Print("Error!\n");
                break;
            case "help":
                Communicator.Print("┌ add <first name> <last name> <address> (don't use only \"_\" symbol)\n");
                Communicator.Print("├ remove <index> (beginning with 1)\n");
                Communicator.Print("├ change <index> <first name> <last name> <address> (if you do not want to change the element, enter \"_\" in the field)\n");
                Communicator.Print("├ swap <index 1> <index 2>\n");
                Communicator.Print("├ show\n");
                Communicator.Print("├ clear (book)\n");
                Communicator.Print("├ sort (by first name)\n");
                Communicator.Print("├ sort <firstname/lastname/address>\n");
                Communicator.Print("├ save <path> <name>\n");
                Communicator.Print("├ open <path> <name (without word 'book/table' in the end)>\n");
                Communicator.Print("└ stop/exit\n");
                break;
            default:
                if (command[0] != "") Communicator.Print($"Don't found command \"{command[0]}\"\n");
                break;
        }

        return 0;
    }
}


interface IDisplayer
{
    void Show(List<PersonInfo> Peoples);
}

class ConsoleDisplayer : IDisplayer
{
    private ICommunicator Communicator;
    public ConsoleDisplayer(ICommunicator communicator)
    {
        Communicator = communicator;
    }
    public void Show(List<PersonInfo> Peoples)
    {
        if (Peoples.Count > 0)
        {

            String result = Book2Table.TableToStr(Peoples);


            Communicator.Print(result);
        }
        else
        {
            Communicator.Print("Address book empty\n");
        }
    }
}

class Book2Table
{
    public static String TableToStr(List<PersonInfo> Peoples)
    {
        int[] spaces;
        String result = TableGenerate(out spaces, Peoples);
        for (int i = 0; i < Peoples.Count; i++)
        {
            PersonInfo person = Peoples[i];
            int spaceId = (int)Math.Log10((double)(i + 1)) + 1;
            int spacedId = spaces[0] - spaceId;
            result += $"│ {StringMethods.Rep(" ", spacedId - spacedId / 2)}{i + 1}{StringMethods.Rep(" ", spacedId / 2)} │ {person.FirstName}{StringMethods.Rep(" ", spaces[1] - (person.FirstName == null ? 0 : person.FirstName.Length))} │ {person.LastName}{StringMethods.Rep(" ", spaces[2] - (person.LastName == null ? 0 : person.LastName.Length))} │ {person.Address}{StringMethods.Rep(" ", spaces[3] - (person.Address == null ? 0 : person.Address.Length))} │\n";
        }
        result += $"└─{StringMethods.Rep("─", spaces[0])}─┴─{StringMethods.Rep("─", spaces[1])}─┴─{StringMethods.Rep("─", spaces[2])}─┴─{StringMethods.Rep("─", spaces[3])}─┘\n";
        return result;
    }
    private static String TableGenerate(out int[] spaces, List<PersonInfo> Peoples)
    {
        String result;
        int maxId = Math.Max((int)Math.Log10((double)Peoples.Count) + 1, 2);
        int maxFirst = 9;
        int maxLast = 8;
        int maxAddress = 7;

        foreach (PersonInfo person in Peoples)
        {
            maxFirst = Math.Max(maxFirst, person.FirstName == null ? 0 : person.FirstName.Length);
            maxLast = Math.Max(maxLast, person.LastName == null ? 0 : person.LastName.Length);
            maxAddress = Math.Max(maxAddress, person.Address == null ? 0 : person.Address.Length);
        }

        spaces = new int[4];
        spaces[0] = maxId;
        spaces[1] = maxFirst;
        spaces[2] = maxLast;
        spaces[3] = maxAddress;

        result = $"┌─{StringMethods.Rep("─", spaces[0])}─┬─{StringMethods.Rep("─", spaces[1])}─┬─{StringMethods.Rep("─", spaces[2])}─┬─{StringMethods.Rep("─", spaces[3])}─┐\n";
        result += $"│ id{StringMethods.Rep(" ", spaces[0] - 2)} │ FirstName{StringMethods.Rep(" ", spaces[1] - 9)} │ LastName{StringMethods.Rep(" ", spaces[2] - 8)} │ Address{StringMethods.Rep(" ", spaces[3] - 7)} │\n";
        result += $"├─{StringMethods.Rep("─", spaces[0])}─┼─{StringMethods.Rep("─", spaces[1])}─┼─{StringMethods.Rep("─", spaces[2])}─┼─{StringMethods.Rep("─", spaces[3])}─┤\n";

        return result;
    }
}

class StringMethods
{
    public static String Rep(String str, int count)
    {
        count = Math.Max(count, 0);
        return String.Concat(Enumerable.Repeat(str, count));
    }
}

struct PersonInfo
{
    public string? FirstName;
    public string? LastName;
    public string? Address;

    public PersonInfo(string? firstName, string? lastName, string? address)
    {
        FirstName = firstName;
        LastName = lastName;
        Address = address;
    }
    public void Validate()
    {
        FirstName = FirstName == "_" ? "None" : FirstName;
        LastName = LastName == "_" ? "None" : LastName;
        Address = Address == "_" ? "None" : Address;
    }
    public void Change(PersonInfo person)
    {
        FirstName = person.FirstName == "_" ? FirstName : person.FirstName;
        LastName = person.LastName == "_" ? LastName : person.LastName;
        Address = person.Address == "_" ? Address : person.Address;
    }
}

class AddressBook
{
    public enum SortBy
    {
        FirstName,
        LastName,
        Address
    }

    private ICommunicator Communicator;
    private IDisplayer Displayer;
    private IBookFileManager BookFileManager;

    private List<PersonInfo> Peoples { get; set; }

    public AddressBook(ICommunicator communicator, IDisplayer displayer, IBookFileManager bookFileManager)
    {
        Peoples = new List<PersonInfo>();
        Communicator = communicator;
        Displayer = displayer;
        BookFileManager = bookFileManager;
    }

    public void Add(PersonInfo person)
    {
        person.Validate();
        Peoples.Add(person);
    }
    public void Remove(int index)
    {
        if (index < Peoples.Count && index >= 0) Peoples.RemoveAt(index);
        else Communicator.Print("Error!!!\n");
    }
    public void Change(int index, PersonInfo person)
    {
        if (index < Peoples.Count && index >= 0)
        {
            PersonInfo per = Peoples[index];
            per.Change(person);
            Peoples[index] = per;
        }
        else Communicator.Print("Error!!!\n");
    }
    public void Swap(int index1, int index2)
    {
        if (index1 < Peoples.Count && index1 >= 0 && index2 < Peoples.Count && index2 >= 0)
        {
            PersonInfo tmp = Peoples[index1];
            Peoples[index1] = Peoples[index2];
            Peoples[index2] = tmp;
        }
        else Communicator.Print("Error!!!\n");
    }
    public void Clear()
    {
        Peoples.Clear();
    }
    public void Show()
    {
        Displayer.Show(Peoples);
    }

    public void Sort(SortBy type, bool descKey)
    {
        switch (type)
        {
            case SortBy.FirstName:
                {
                    Peoples = Peoples.OrderBy(x => x.FirstName).ToList();
                    break;
                }
            case SortBy.LastName:
                {
                    Peoples = Peoples.OrderBy(x => x.LastName).ToList();
                    break;
                }
            case SortBy.Address:
                {
                    Peoples = Peoples.OrderBy(x => x.Address).ToList();
                    break;
                }
        }
        if (descKey) Peoples.Reverse();

    }

    public void Sort(bool descKey)
    {
        Sort(SortBy.Address, descKey);
        Sort(SortBy.LastName, descKey);
        Sort(SortBy.FirstName, descKey);
    }

    public void Sort(SortBy type1, SortBy type2, bool descKey)
    {
        Sort(type2, descKey);
        Sort(type1, descKey);
    }

    public void Sort(SortBy type1, SortBy type2, SortBy type3, bool descKey)
    {
        Sort(type3, descKey);
        Sort(type2, descKey);
        Sort(type1, descKey);
    }

    public void Open(String path, String name)
    {
        Peoples = BookFileManager.Open(path, name);
    }

    public void Save(String path, String name)
    {
        BookFileManager.Save(path, name, Peoples);
    }
}

interface IBookFileSaver
{
    void Save(String path, String name, List<PersonInfo> Peoples);
}
interface IBookFileOpener
{
    List<PersonInfo> Open(String path, String name);
}

interface IBookFileManager : IBookFileOpener, IBookFileSaver { }

class BookFileManagerAStxt : IBookFileManager
{
    private ICommunicator Communicator;
    public BookFileManagerAStxt(ICommunicator communicator)
    {
        Communicator = communicator;
    }
    public void Save(String path, String name, List<PersonInfo> Peoples)
    {
        try
        {
            using (StreamWriter sw = File.CreateText(path + @"\" + name + @"_book.txt"))
            {
                foreach (PersonInfo person in Peoples)
                {
                    sw.Write($"{person.FirstName} {person.LastName} {person.Address}\n");
                }
            }
            using (StreamWriter sw = File.CreateText(path + @"\" + name + @"_table.txt"))
            {
                sw.Write(Book2Table.TableToStr(Peoples));
            }
        }
        catch
        {
            Communicator.Print("Error!");
        }
    }

    public List<PersonInfo> Open(String path, String name)
    {
        try
        {
            List<PersonInfo> Peoples = new List<PersonInfo>();
            using (StreamReader sw = new StreamReader(path + @"\" + name + @"_book.txt"))
            {
                String? ln;
                while ((ln = sw.ReadLine()) != null)
                {
                    String[] info = ln.Split();
                    Peoples.Add(new PersonInfo(info[0], info[1], info[2]));
                }
            }
            return Peoples;
        }
        catch
        {
            Communicator.Print("Error!");
            return new List<PersonInfo>();
        }
    }
}
