Console.WriteLine(@"Address Book by
 ____                                                                       _____              
|  _ \  _ __   ___    __ _  _ __   __ _  _ __ ___   _ __ ___    ___  _ __  |  ___|  ___  __  __
| |_) || '__| / _ \  / _` || '__| / _` || '_ ` _ \ | '_ ` _ \  / _ \| '__| | |_    / _ \ \ \/ /
|  __/ | |   | (_) || (_| || |   | (_| || | | | | || | | | | ||  __/| |    |  _|  | (_) | >  < 
|_|    |_|    \___/  \__, ||_|    \__,_||_| |_| |_||_| |_| |_| \___||_|    |_|     \___/ /_/\_\
                     |___/                                                                     ");
Console.WriteLine("Commands: \"add\", \"remove\", \"show\", \"clear\" (book), \"save\", \"open\", \"help\", \"stop\"\n");

AddressBook addressBook = new AddressBook();
addressBook.handle += DisplayPeoples;

while (true)
{
    Console.Write("Command: ");
    String? line = Console.ReadLine();
    if (line == null) continue;
    String[] command = line.Split();
    if (command[0].ToLower() == "add")
    {
        if(command.Length == 4) addressBook.Add(new PersonInfo(command[1], command[2], command[3]));
        else Console.WriteLine("Error!");
    }
    else if(command[0].ToLower() == "show")
    {
        addressBook.Show();
    }
    else if(command[0].ToLower() == "remove")
    {
        int num;
        if (int.TryParse(command[1], out num)) addressBook.Remove(num);
        else Console.WriteLine("Error!");
    }
    else if (command[0].ToLower() == "clear")
    {
        addressBook.Clear();
    }
    else if (command[0].ToLower() == "stop")
    {
        break;
    }
    else if(command[0].ToLower() == "save")
    {
        if(command.Length == 3) addressBook.Save(command[1], command[2]);
        else Console.WriteLine("Error!");

    }
    else if (command[0].ToLower() == "open")
    {
        if (command.Length == 3) addressBook.Open(command[1], command[2]);
        else Console.WriteLine("Error!");

    }
    else if (command[0].ToLower() == "help")
    {
        Console.WriteLine("┌ add <first name> <last name> <address>");
        Console.WriteLine("├ remove <index> (beginning with 1)");
        Console.WriteLine("├ show");
        Console.WriteLine("├ clear");
        Console.WriteLine("├ save <path> <name>");
        Console.WriteLine("├ open <path> <name>");
        Console.WriteLine("└ stop");
    }
    else
    {
        if(command[0] != "") Console.WriteLine($"Don't found command \"{command[0]}\"");
    }
}



void DisplayPeoples(String Message)
{
    Console.WriteLine(Message);
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
}

class AddressBook
{
    public delegate void PlayerHandle(String Message);
    public event PlayerHandle? handle = null;

    private List<PersonInfo> Peoples { get; set; }

    public AddressBook()
    {
        Peoples = new List<PersonInfo>();
    }

    public void Add(PersonInfo person)
    {
        Peoples.Add(person);
    }
    public void Remove(int index)
    {
        index -= 1;
        if(index < Peoples.Count && index >= 0) Peoples.RemoveAt(index);
        else handle?.Invoke("Error!!!");
    }
    public void Clear()
    {
        Peoples.Clear();
    }
    public void Show()
    {
        if (Peoples.Count > 0)
        {

            String result = TableToStr();


            handle?.Invoke(result);
        }
        else
        {
            handle?.Invoke("Address book empty");
        }
    }

    private String TableToStr()
    {
        int[] spaces;
        String result = TableGenerate(out spaces);
        for (int i = 0; i < Peoples.Count; i++)
        {
            PersonInfo person = Peoples[i];
            int spaceId = (int)Math.Log10((double)(i + 1)) + 1;
            int spacedId = spaces[0] - spaceId;
            result += $"│ {Rep(" ", spacedId - spacedId / 2)}{i + 1}{Rep(" ", spacedId / 2)} │ {person.FirstName}{Rep(" ", spaces[1] - (person.FirstName == null ? 0 : person.FirstName.Length))} │ {person.LastName}{Rep(" ", spaces[2] - (person.LastName == null ? 0 : person.LastName.Length))} │ {person.Address}{Rep(" ", spaces[3] - (person.Address == null ? 0 : person.Address.Length))} │\n";
        }
        result += $"└─{Rep("─", spaces[0])}─┴─{Rep("─", spaces[1])}─┴─{Rep("─", spaces[2])}─┴─{Rep("─", spaces[3])}─┘\n";
        return result;
    }


    public void Save(String path, String name)
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
                sw.Write(TableToStr());
            }
        }
        catch
        {
            handle?.Invoke("Error!");
        }
    }

    public void Open(String path, String name)
    {
        try
        {
            Clear();
            using (StreamReader sw = new StreamReader(path + @"\" + name + @"_book.txt"))
            {
                String? ln;
                while ((ln = sw.ReadLine()) != null)
                {
                    String[] info = ln.Split();
                    Add(new PersonInfo(info[0], info[1], info[2]));
                }
            }
        }
        catch
        {
            handle?.Invoke("Error!");
        }
    }


    private String TableGenerate(out int[] spaces)
    {
        String result  = "";
        int maxId      = Math.Max((int)Math.Log10((double)Peoples.Count) + 1, 2);
        int maxFirst   = 9;
        int maxLast    = 8;
        int maxAddress = 7;

        foreach (PersonInfo person in Peoples)
        {
            maxFirst   = Math.Max(maxFirst,   person.FirstName == null ? 0 : person.FirstName.Length);
            maxLast    = Math.Max(maxLast,    person.LastName  == null ? 0 : person.LastName.Length);
            maxAddress = Math.Max(maxAddress, person.Address   == null ? 0 : person.Address.Length);
        }

        spaces = new int[4];
        spaces[0] = maxId;
        spaces[1] = maxFirst;
        spaces[2] = maxLast;
        spaces[3] = maxAddress;

        result = $"┌─{Rep("─", spaces[0])}─┬─{Rep("─", spaces[1])}─┬─{Rep("─", spaces[2])}─┬─{Rep("─", spaces[3])}─┐\n";
        result += $"│ id{ Rep(" ", spaces[0] - 2) } │ FirstName{ Rep(" ", spaces[1] - 9) } │ LastName{ Rep(" ", spaces[2] - 8) } │ Address{ Rep(" ", spaces[3] - 7) } │\n";
        result += $"├─{Rep("─", spaces[0])}─┼─{Rep("─", spaces[1])}─┼─{Rep("─", spaces[2])}─┼─{Rep("─", spaces[3])}─┤\n";

        return result;
    }

    private String Rep(String str, int count)
    {
        count = Math.Max(count, 0);
        return String.Concat(Enumerable.Repeat(str, count));
    }
}