# Documenta��o `(RASCUNHO)`

Segue umas pequenas notas do que se pretende com esse projeto.

## No��es b�sicas

### CommandLineApp m�nima

```csharp
var cmdApp = new CommandLineApp("cmd", "My Command Line App");

cmdApp.PrintUsage();
```
_@output:_
```
cmd - My Command Line App

USAGE:
    $ cmd [<args>]
```

### CommandLineApp com vers�o

```csharp
var cmdApp = new CommandLineApp("cmd", "My Command Line App");

cmdApp.Version("1.0.0-alpha-67890");

cmdApp.PrintUsage();
```
_@output:_
```
cmd - My Command Line App [version 1.0.0-alpha-67890]

USAGE:
    $ cmd [<args>]
```

### CommandLineApp com direitos autorais

```csharp
var cmdApp = new CommandLineApp("cmd", "My Command Line App");

cmdApp.Copyright("(c) 1991-2018 E5R Development Team. All rights reserved.");

cmdApp.PrintHelp();
```
_@output:_
```
cmd - My Command Line App
Copyright (c) 1991-2018 E5R Development Team. All rights reserved.

USAGE:
    $ cmd [<args>]
```

### CommandLineApp com sinopse

```csharp
var cmdApp = new CommandLineApp("cmd", "My Command Line App");

cmdApp.Synopsis(new string[]
{
    "This is a my command line app to exemplify the use of this",
    "exceptional library. In the next few lines you'll see how",
    "easy it is to write a command line program."
});

cmdApp.PrintHelp();
```
_@output:_
```
cmd - My Command Line App

This is a my command line app to exemplify the use of this
exceptional library. In the next few lines you'll see how
easy it is to write a command line program.

USAGE:
    $ cmd [<args>]
```

### CommandLineApp mais completo

```csharp
var cmdApp = new CommandLineApp("cmd", "My Command Line App")

    .Version("1.0.0-alpha-67890")
    .Copyright("(c) 1991-2018 E5R Development Team. All rights reserved.")
    .Synopsis(new string[] {
        "This is a my command line app to exemplify the use of this",
        "exceptional library. In the next few lines you'll see how",
        "easy it is to write a command line program." });

cmdApp.PrintUsage();
```
_@output:_
```
cmd - My Command Line App [version 1.0.0-alpha-67890]

USAGE:
    $ cmd [<args>]
```

```csharp
cmdApp.PrintHelp();
```
_@output:_
```
cmd - My Command Line App [version 1.0.0-alpha-67890]
Copyright (c) 1991-2018 E5R Development Team. All rights reserved.

This is a my command line app to exemplify the use of this
exceptional library. In the next few lines you'll see how
easy it is to write a command line program.

USAGE:
    $ cmd [<args>]
```

### Definindo e usando OPTION's

```csharp
var cmdApp = new CommandLineApp("cmd", "My Command Line App");

cmdApp.Option("Action", "The app action")
      .Template("-a|--action");

var args = new string[] { "-a","My Action Value" };
                  // or { "-a=""My Action Value""" };
                  // or { "--action","My Action Value" };
                  // or { "--action=""My Action Value""" };

CommandLineOptions options = cmdApp.Parse(args);

Assert.True(options.Defined("Action"));
Assert.Equal("My Action Value", options.ValueOf("Action"));

Assert.False(options.Defined("NotFoundAction"));

cmdApp.PrintHelp();
```
_@output:_
```
cmd - My Command Line App

USAGE:
    $ cmd [<options>] [<args>]

OPTIONS:
    -a|--action <Action>        The app action
```

Como visto acima, podemos informar uma op��o de v�rias formas, veja:

```powershell
$ program -a "My Action Value"
# { "-a", "My Action Value" }

$ program -a="My Action Value"
# { "-a="My Action Value" }

$ program --action "My Action Value"
# { "--action", "My Action Value" }

$ program --action="My Action Value"
# { "--action=""My Action Value""" }
```

Mas para fins did�ticos, usaremos sempre a mesma forma abaixo para simplificar
o entendimento (por�m voc� est� livre para usar a forma que preferir):

```powershell
$ program --action "My Action Value"
# { "--action", "My Action Value" }
```

### Definindo e usando FLAG's

```csharp
var cmdApp = new CommandLineApp("cmd", "My Command Line App");

cmdApp.Flag("Verbose", "Print verbose messages")
      .Template("-v|--verbose");

var args = new string[] { "--verbose" };

CommandLineOptions options = cmdApp.Parse(args);

Assert.True(options.Defined("Verbose"));
Assert.False(options.Defined("NotFoundFlag"));

cmdApp.Printhelp();
```
_@output:_
```
cmd - My Command Line App

USAGE:
    $ cmd [<flags>] [<args>]

FLAGS:
    -v|--verbose                Print verbose messages
```

### Definindo e usando OPTION'S e FLAG's juntas

```csharp
var cmdApp = new CommandLineApp("cmd", "My Command Line App");

cmdApp.Option("Action", "The app action")
      .Template("-a|--action");

cmdApp.Flag("Verbose", "Print verbose messages")
      .Template("-v|--verbose");

var args = new string[] { "-va", "My Action Value" };

CommandLineOptions options = cmdApp.Parse(args);

Assert.True(options.Defined("Action"));
Assert.Equal("My Action Value", options.ValueOf("Action"));
Assert.False(options.Defined("NotFoundAction"));

Assert.True(options.Defined("Verbose"));
Assert.False(options.Defined("NotFoundFlag"));

cmdApp.PrintHelp();
```
_@output:_
```
cmd - My Command Line App

USAGE:
    $ cmd [<options>|<flags>] [<args>]

OPTIONS:
    -a|--action <Action>        The app action

FLAGS:
    -v|--verbose                Print verbose messages
```

**OBS:** Neste momento estamos usando a forma _curta_ dos argumentos.

### Usando argumentos

Voc� define suas **OPTION**'s e **FLAG**'s, e tudo que for passado para a
linha de comando que n�o for uma **option** ou **flag** � um...

**� UM ARGUMENTO!**

Veja como usar abaixo.

```csharp
var cmdApp = new CommandLineApp(...);

cmdApp.Option("Action", "The app action")
      .Template("-a|--action");

cmdApp.Flag("Verbose", "Print verbose messages")
      .Template("-v|--verbose");

var args = new string[] { "-va", "My Action Value", "Argument value", "--other" };

CommandLineOptions options = cmdApp.Parse(args);

Assert.NotEmpty(options.Arguments);
Assert.Equal(2, options.Arguments.Length);

Assert.DoesNotContain("-va", options.Arguments);
Assert.DoesNotContain("My Action Value", options.Arguments);

Assert.Contains("--other", options.Arguments);
Assert.Contains("My Action Value", options.Arguments);
```

**OBS:** Neste momento estamos usando a forma _curta_ dos argumentos.

## No��es mais elegantes

Al�m de usar `CommandLineApp` para configurar sua aplica��o de forma expl�cita
como vimos at� agora, uma outra forma de fazer isso � usando anota��es.

Na verdade essa � a forma que prefixo e recomendo para configurar sua `CommandLineApp`.

Como normalmente um programa de linha de comando tem a famosa classe `Program` (n�o
que isso seja obrigat�rio), essa � por padr�o a descri��o de nossa aplica��o.

E usando as anota��es podemos **"efeit�-la"** para corresponder nossa aplica��o mais **BELA**.

Nas linhas abaixo vamos repetir os exemplos que j� mostramos at� agora, por�m usanso
anota��es ao inv�s da **API Fluente**.

### CommandLineApp m�nima `(com Annotations)`

```csharp
class Program : CommandLineWrapper
{
    Program(string[] args) : base("cmd", "My Command Line App")
    {
        PrintUsage();
    }

    static void Main(string[] args) => new Program(args);
}
```
_@output:_
```
cmd - My Command Line App

USAGE:
    $ cmd [<args>]
```

> **PS:** Observe que omitimos os modificadores de acesso  (`public`, `protected`, 
> `private`) por quest�es did�ticas e pra facilitar a leitura.

### CommandLineApp com vers�o `(com Annotations)`

```csharp
class Program : CommandLineWrapper
{
    Program(string[] args) : base("cmd", "My Command Line App")
    {
        Version = "1.0.0-alpha-67890";

        PrintUsage();
    }

    static void Main(string[] args) => new Program(args);
}
```
_@output:_
```
cmd - My Command Line App [version 1.0.0-alpha-67890]

USAGE:
    $ cmd [<args>]
```

### CommandLineApp com direitos autorais `(com Annotations)`

```csharp
class Program : CommandLineWrapper
{
    Program(string[] args) : base("cmd", "My Command Line App")
    {
        Copyright = "(c) 1991-2018 E5R Development Team. All rights reserved.";

        PrintHelp();
    }

    static void Main(string[] args) => new Program(args);
}
```
_@output:_
```
cmd - My Command Line App
Copyright (c) 1991-2018 E5R Development Team. All rights reserved.

USAGE:
    $ cmd [<args>]
```

### CommandLineApp com sinopse `(com Annotations)`

```csharp
class Program : CommandLineWrapper
{
    override string[] Synopsis { get; set; } = {
        "This is a my command line app to exemplify the use of this",
        "exceptional library. In the next few lines you'll see how",
        "easy it is to write a command line program."
    };

    Program(string[] args) : base("cmd", "My Command Line App")
    {
        PrintHelp();
    }

    static void Main(string[] args) => new Program(args);
}
```
_@output:_
```
cmd - My Command Line App

This is a my command line app to exemplify the use of this
exceptional library. In the next few lines you'll see how
easy it is to write a command line program.

USAGE:
    $ cmd [<args>]
```

Observe que aqui n�s usamos o recurso de sobrescrita para a propriedade `Synopsis`,
mas voc� tamb�m poderia t�-la atribu�do no construtor da mesma forma que fez com
`Version` e `Copyright`; bem como, tamb�m pode usar o recurso de sobrescrita
para essas propriedades tamb�m.

### CommandLineApp mais completo `(com Annotations)`

```csharp
class Program : CommandLineWrapper
{
    override string[] Synopsis { get; set; } = {
        "This is a my command line app to exemplify the use of this",
        "exceptional library. In the next few lines you'll see how",
        "easy it is to write a command line program."
    };

    Program(string[] args) : base("cmd", "My Command Line App")
    {
        Version = "1.0.0-alpha-67890";
        Copyright = "(c) 1991-2018 E5R Development Team. All rights reserved.";

        PrintUsage();
    }

    static void Main(string[] args) => new Program(args);
}
```
_@output:_
```
cmd - My Command Line App [version 1.0.0-alpha-67890]

USAGE:
    $ cmd [<args>]
```

```csharp
class Program
{
    // ...

    Program()
    {
        // ...

        PrintHelp();
    }
}
```
_@output:_
```
cmd - My Command Line App [version 1.0.0-alpha-67890]
Copyright (c) 1991-2018 E5R Development Team. All rights reserved.

This is a my command line app to exemplify the use of this
exceptional library. In the next few lines you'll see how
easy it is to write a command line program.

USAGE:
    $ cmd [<args>]
```

### Definindo e usando OPTION's `(com Annotations)`

```csharp
class Program : CommandLineWrapper
{
    [OptionWrapper("-a|--action", "The app action")]
    string Action { get; set; }

    Program(string[] args) : base("cmd", "My Command Line App")
    {
        Parse(args);

        Assert.NotNull(Action);
        Assert.NotEmpty(Action);
        Assert.Equal("My Action Value", Action);
        Assert.True(Defined(nameof(Action)));
        Assert.False(Defined("NotFoundAction"));

        PrintHelp();
    }

    // @args: { "--action", "My Action Value" }
    static void Main(string[] args) => new Program(args);
}
```
_@output:_
```
cmd - My Command Line App

USAGE:
    $ cmd [<options>] [<args>]

OPTIONS:
    -a|--action <Action>        The app action
```

### Definindo e usando FLAG's `(com Annotations)`

```csharp
class Program : CommandLineWrapper
{
    [FlagWrapper("-v|--verbose", "Print verbose messages")]
    string Verbose { get; set; }

    Program(string[] args) : base("cmd", "My Command Line App")
    {
        Parse(args);

        Assert.True(Verbose);
        Assert.True(Defined(nameof(Verbose)));
        Assert.False(Defined("NotFoundFlag"));

        PrintHelp();
    }

    // @args: { "--verbose" }
    static void Main(string[] args) => new Program(args);
}
```
_@output:_
```
cmd - My Command Line App

USAGE:
    $ cmd [<flags>] [<args>]

FLAGS:
    -v|--verbose                Print verbose messages
```

### Definindo e usando OPTION'S e FLAG's juntas `(com Annotations)`

```csharp
class Program : CommandLineWrapper
{
    [OptionWrapper("-a|--action", "The app action")]
    string Action { get; set; }

    [FlagWrapper("-v|--verbose", "Print verbose messages")]
    string Verbose { get; set; }

    Program(string[] args) : base("cmd", "My Command Line App")
    {
        Parse(args);

        Assert.True(Defined(nameof(Action)));
        Assert.NotNull(Action);
        Assert.NotEmpty(Action);
        Assert.Equal("My Action Value", Action);
        Assert.False(Defined("NotFoundAction"));

        Assert.True(Verbose);
        Assert.True(Defined(nameof(Verbose)));
        Assert.False(Defined("NotFoundFlag"));

        PrintHelp();
    }

    // @args: { "-va", "My Action Value" }
    static void Main(string[] args) => new Program(args);
}
```
_@output:_
```
cmd - My Command Line App

USAGE:
    $ cmd [<options>|<flags>] [<args>]

OPTIONS:
    -a|--action <Action>        The app action

FLAGS:
    -v|--verbose                Print verbose messages
```

### Usando argumentos `(com Annotations)`

```csharp
class Program : CommandLineWrapper
{
    [OptionWrapper("-a|--action", "The app action")]
    string Action { get; set; }

    [FlagWrapper("-v|--verbose", "Print verbose messages")]
    string Verbose { get; set; }

    Program(string[] args) : base("cmd", "My Command Line App")
    {
        Parse(args);

        Assert.NotEmpty(Arguments);
        Assert.Equal(2, Arguments.Length);

        Assert.DoesNotContain("-va", Arguments);
        Assert.DoesNotContain("My Action Value", Arguments);

        Assert.Contains("--other", Arguments);
        Assert.Contains("My Action Value", Arguments);
    }

    // @args: { "-va", "My Action Value", "Argument value", "--other" }
    static void Main(string[] args) => new Program(args);
}
```
