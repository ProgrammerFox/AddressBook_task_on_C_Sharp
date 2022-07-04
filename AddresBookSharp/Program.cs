Console.WriteLine(@"Address Book by
 ____                                                                       _____              
|  _ \  _ __   ___    __ _  _ __   __ _  _ __ ___   _ __ ___    ___  _ __  |  ___|  ___  __  __
| |_) || '__| / _ \  / _` || '__| / _` || '_ ` _ \ | '_ ` _ \  / _ \| '__| | |_    / _ \ \ \/ /
|  __/ | |   | (_) || (_| || |   | (_| || | | | | || | | | | ||  __/| |    |  _|  | (_) | >  < 
|_|    |_|    \___/  \__, ||_|    \__,_||_| |_| |_||_| |_| |_| \___||_|    |_|     \___/ /_/\_\
                     |___/                                                                     ");
Console.WriteLine("Commands: \"add\", \"remove\", \"change\", \"swap\", \"show\", \"clear\", \"sort\", \"save\", \"open\", \"help\", \"stop/exit\"\n");

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
        if (command.Length == 4) addressBook.Add(new PersonInfo(command[1], command[2], command[3]));
        else Console.WriteLine("Error!");
    }
    else if (command[0].ToLower() == "change")
    {
        int num;
        if (int.TryParse(command[1], out num) && command.Length == 5) addressBook.Change(num, new PersonInfo(command[2], command[3], command[4]));
        else Console.WriteLine("Error!");
    }
    else if (command[0].ToLower() == "swap")
    {
        int index1, index2;
        if (int.TryParse(command[1], out index1) && int.TryParse(command[2], out index2) && command.Length == 3) addressBook.Swap(index1, index2);
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
    else if (command[0].ToLower() == "sort")
    {
        if(command.Length == 1) addressBook.Sort();
        else if (command.Length == 2)
        {
            if(command[1].ToLower()      == "firstname") addressBook.Sort(AddressBook.SortBy.FirstName);
            else if(command[1].ToLower() == "lastname")  addressBook.Sort(AddressBook.SortBy.LastName);
            else if(command[1].ToLower() == "address")   addressBook.Sort(AddressBook.SortBy.Address);
            else Console.WriteLine("Error!");
        }
        else Console.WriteLine("Error!");
    }
    else if (command[0].ToLower() == "stop" || command[0].ToLower() == "exit")
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
        Console.WriteLine("┌ add <first name> <last name> <address> (don't use only \"_\" symbol)");
        Console.WriteLine("├ remove <index> (beginning with 1)");
        Console.WriteLine("├ change <index> <first name> <last name> <address> (if you do not want to change the element, enter \"_\" in the field)");
        Console.WriteLine("├ swap <index 1> <index 2>");
        Console.WriteLine("├ show");
        Console.WriteLine("├ clear (book)");
        Console.WriteLine("├ sort (by first name)");
        Console.WriteLine("├ sort <firstname/lastname/address>");
        Console.WriteLine("├ save <path> <name>");
        Console.WriteLine("├ open <path> <name>");
        Console.WriteLine("└ stop/exit");
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
        LastName  = lastName;
        Address   = address;
    }
    public void Validate()
    {
        FirstName = FirstName == "_" ? "None" : FirstName;
        LastName  = LastName  == "_" ? "None" : LastName;
        Address   = Address   == "_" ? "None" : Address;
    }
    public void Change(PersonInfo person)
    {
        FirstName = person.FirstName == "_" ? FirstName : person.FirstName;
        LastName  = person.LastName  == "_" ? LastName  : person.LastName;
        Address   = person.Address   == "_" ? Address   : person.Address;
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

    public delegate void PlayerHandle(String Message);
    public event PlayerHandle? handle = null;

    private List<PersonInfo> Peoples { get; set; }

    public AddressBook()
    {
        Peoples = new List<PersonInfo>();
    }

    public void Add(PersonInfo person)
    {
        person.Validate();
        Peoples.Add(person);
    }
    public void Remove(int index)
    {
        index -= 1;
        if(index < Peoples.Count && index >= 0) Peoples.RemoveAt(index);
        else handle?.Invoke("Error!!!");
    }
    public void Change(int index, PersonInfo person)
    {
        index -= 1;
        if (index < Peoples.Count && index >= 0)
        {
            PersonInfo per = Peoples[index];
            per.Change(person);
            Peoples[index] = per;
        } 
        else handle?.Invoke("Error!!!");
    }
    public void Swap(int index1, int index2)
    {
        index1 -= 1;
        index2 -= 1;
        if (index1 < Peoples.Count && index1 >= 0 && index2 < Peoples.Count && index2 >= 0)
        {
            PersonInfo tmp = Peoples[index1];
            Peoples[index1] = Peoples[index2];
            Peoples[index2] = tmp;
        }
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
    public void Sort()
    {
        Peoples = Peoples.OrderBy(x => x.FirstName).ToList();
    }
    public void Sort(SortBy type)
    {
        switch(type)
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