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

## No��es avan�adas

Vamos agora falar de algumas _coisinhas_ que n�o foram colocadas nos exemplos
anteriores pra n�o confundir quem est� aprendendo agora. Por�m falaremos agora,
e isso � s� porque voc� j� sabe o b�sico (se n�o sabe, sugiro estudar os t�picos
anteriores).

## M�todos print

1. N�o imprimimos diretamente no console

Entendo que a aplica��o em si � na verdade a respons�vel por decidir qual a melhor
forma de imprimir no console, ou usando _ILog interfaces_ ou o que preferir.

Assim, em nossos m�todos de impress�o (`PrintHelp()`, `PrintUsage()` - e ainda
tem o n�o mencionado `PrintVersion()`) na verdade a �nica coisa que acontece �
que `CommandLineApp` formata a mensagem que precisa ser impressa e repassa isso
a um `Delegate` definido pela aplica��o. Se a aplica��o n�o define um `Delegate`
a mensagem simplesmente **n�o vai ser apresentada**.

Voc� pode fazer isso usando **API Fluente**:

```csharp
cmdApp.OnPrint(string format, params object[] args)
    => Console.WriteLine(format, args));
```

Ou em sua classe **Program**:

```csharp
class Program : CommandLineWrapper
{
    Program(string[] args) : base("cmd", "My Command Line App")
    {
        PrintUsage();
    }

    override void OnPrint(string format, params object[] args)
        => Console.WriteLine(format, args);

    static void Main(string[] args) => new Program(args);
}
```

2. O m�todo `PrintVersion()` que falamos antes

Com **API Fluente**:

```csharp
var cmdApp = new CommandLineApp("cmd", "My Command Line App");

cmdApp.Version("1.0.0-alpha-67890");

cmdApp.PrintVersion();
```
_@output:_
```
1.0.0-alpha-67890
```

Com classe **Program**:

```csharp
class Program : CommandLineWrapper
{
    Program(string[] args) : base("cmd", "My Command Line App")
    {
        Version = "1.0.0-alpha-67890";

        PrintVersion();
    }

    static void Main(string[] args) => new Program(args);
}
```
_@output:_
```
1.0.0-alpha-67890
```

### Argumentos nomeados

Com **API Fluente**:

```csharp
var cmdApp = new CommandLineApp("cmd", "My Command Line App")

    .NamedArgument("Source", "The source file path", required: true)
    .NamedArgument("Target", "The target file path", required: false);

cmdApp.PrintUsage();
```
_@output:_
```
cmd - My Command Line App

USAGE:
    $ cmd <Source> [<Target>|<args>]
```

```csharp
cmdApp.PrintHelp();
```
_@output:_
```
cmd - My Command Line App

USAGE:
    $ cmd <Source> [<Target>|<args>]

ARGS:
    <Source*>                   The source file path
    <Target>                    The target file path

(*) Argument is required
```

Com classe **Program**:

```csharp
class Program : CommandLineWrapper
{
    [NamedArgumentWrapper("The source file path", required: true)]
    string Source { get; set; }

    [NamedArgumentWrapper("The target file path")]
    string Target { get; set; }

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
    $ cmd <Source> [<Target>|<args>]
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
cmd - My Command Line App

USAGE:
    $ cmd <Source> [<Target>|<args>]

ARGS:
    <Source*>                   The source file path
    <Target>                    The target file path

(*) Argument is required
```

### EnsuresHelpInformation()

N�o usamos `PrintHelp()` diretamente, mas chamamos o m�todo `EnsuresHelpInformation()`,
esse que verifica se o usu�rio informou `-h?|--help`, se sim, exibe a mensagem de ajuda
e retorna `TRUE`, com o resultado o programa decide o que fazer, se para, continua,
ou "sei l� o que".

O usu�rio tamb�m pode definir o padr�o para a **flag** de ajuda:

Usando **API Fluente**

```csharp
cmdApp.HelpFlag
      .Pattern("-h?|--help|--show-help")
      .Description("Show this help message!");
```

Com classe **Program**:

```csharp
class Program : CommandLineWrapper
{
    [FlagWrapper("-h?|--help|--show-help", "Show this help message!")]
    override bool Help { get; set; }

    // ...
}
```

### EnsuresUsageInformation()

Este m�todo � semelhante ao `EnsuresHelpInformation()`, por�m sua responsabilidade
� garantir que os par�metros obrigat�rios foram informados, e que os `Wrapper's`
tiveram seus valores convertidos adequadamente para as propriedades com `annotations`.

Quando o m�todo `Parse()` � chamado, ele configura a aplica��o e tenta fazer o `bind`
dos dados dos par�metros para suas propriedades. Neste momento, se algum erro for
encontrado, ao inv�s de levantar uma exce��o imediatamente ele **guarda** essas
exce��es e salva um estado de erro.

Quando chamamos `EnsuresUsageInformation()` estamos na verdade querendo garantir que,
se algum erro ocorreu, n�s iremos apresentar a mensagem de **`USAGE!`**.

Se houve algum erro, este m�todo retorna `TRUE`, a� o programa decide o que fazer.

Este m�todo � silencioso propositalmente. Caso voc� queira apresentar os erros
encontrados, a� voc� deve ler o pr�ximo item `EnsureErrorInformation()`.

### EnsuresErrorInformation()

Este m�todo � basicamente um atalho para `EnsuresUsageInformation()`, por�m com
a garantia que os erros ser�o primeiramente apresentados, antes da informa��o **`USAGE!`**.

E a impress�o dos erros tamb�m s� ocorre se o `Deletage` de Log de erros for
configurado, assim como `OnPrint Delegate`:

Voc� pode fazer isso usando **API Fluente**:

```csharp
cmdApp.OnError(CommandLineErrorType typeError, CommandLineException exception)
    => Console.WriteLine("{0}: {1}", typeError, exception));
```

Ou em sua classe **Program**:

```csharp
class Program : CommandLineWrapper
{
    Program(string[] args) : base("cmd", "My Command Line App")
    {
        EnsuresErrorInformation();
    }

    override void OnError(CommandLineErrorType typeError, CommandLineException exception)
        => Console.WriteLine("{0}: {1}", typeError, exception));

    static void Main(string[] args) => new Program(args);
}

enum CommandLineErrorType 
{
    // TODO: ...
}
```