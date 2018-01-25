# Documentação `(RASCUNHO)`

Segue umas pequenas notas do que se pretende com esse projeto.

## Noções básicas

### CommandLineApp mínima

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

### CommandLineApp com versão

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

Como visto acima, podemos informar uma opção de várias formas, veja:

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

Mas para fins didáticos, usaremos sempre a mesma forma abaixo para simplificar
o entendimento (porém você está livre para usar a forma que preferir):

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

Você define suas **OPTION**'s e **FLAG**'s, e tudo que for passado para a
linha de comando que não for uma **option** ou **flag** é um...

**É UM ARGUMENTO!**

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

## Noções mais elegantes

Além de usar `CommandLineApp` para configurar sua aplicação de forma explícita
como vimos até agora, uma outra forma de fazer isso é usando anotações.

Na verdade essa é a forma que prefixo e recomendo para configurar sua `CommandLineApp`.

Como normalmente um programa de linha de comando tem a famosa classe `Program` (não
que isso seja obrigatório), essa é por padrão a descrição de nossa aplicação.

E usando as anotações podemos **"efeitá-la"** para corresponder nossa aplicação mais **BELA**.

Nas linhas abaixo vamos repetir os exemplos que já mostramos até agora, porém usanso
anotações ao invés da **API Fluente**.

### CommandLineApp mínima `(com Annotations)`

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
> `private`) por questões didáticas e pra facilitar a leitura.

### CommandLineApp com versão `(com Annotations)`

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

Observe que aqui nós usamos o recurso de sobrescrita para a propriedade `Synopsis`,
mas você também poderia tê-la atribuído no construtor da mesma forma que fez com
`Version` e `Copyright`; bem como, também pode usar o recurso de sobrescrita
para essas propriedades também.

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

## Noções avançadas

Vamos agora falar de algumas _coisinhas_ que não foram colocadas nos exemplos
anteriores pra não confundir quem está aprendendo agora. Porém falaremos agora,
e isso é só porque você já sabe o básico (se não sabe, sugiro estudar os tópicos
anteriores).

### Métodos print

1. Não imprimimos diretamente no console

Entendo que a aplicação em si é na verdade a responsável por decidir qual a melhor
forma de imprimir no console, ou usando _ILog interfaces_ ou o que preferir.

Assim, em nossos métodos de impressão (`PrintHelp()`, `PrintUsage()` - e ainda
tem o não mencionado `PrintVersion()`) na verdade a única coisa que acontece é
que `CommandLineApp` formata a mensagem que precisa ser impressa e repassa isso
a um `Delegate` definido pela aplicação. Se a aplicação não define um `Delegate`
a mensagem simplesmente **não vai ser apresentada**.

Você pode fazer isso usando **API Fluente**:

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

2. O método `PrintVersion()` que falamos antes

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

Não usamos `PrintHelp()` diretamente, mas chamamos o método `EnsuresHelpInformation()`,
esse que verifica se o usuário informou `-h?|--help`, se sim, exibe a mensagem de ajuda
e retorna `TRUE`, com o resultado o programa decide o que fazer, se para, continua,
ou "sei lá o que".

O usuário também pode definir o padrão para a **flag** de ajuda:

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

Este método é semelhante ao `EnsuresHelpInformation()`, porém sua responsabilidade
é garantir que os parâmetros obrigatórios foram informados, e que os `Wrapper's`
tiveram seus valores convertidos adequadamente para as propriedades com `annotations`.

Quando o método `Parse()` é chamado, ele configura a aplicação e tenta fazer o `bind`
dos dados dos parâmetros para suas propriedades. Neste momento, se algum erro for
encontrado, ao invés de levantar uma exceção imediatamente ele **guarda** essas
exceções e salva um estado de erro.

Quando chamamos `EnsuresUsageInformation()` estamos na verdade querendo garantir que,
se algum erro ocorreu, nós iremos apresentar a mensagem de **`USAGE!`**.

Se houve algum erro, este método retorna `TRUE`, aí o programa decide o que fazer.

Este método é silencioso propositalmente. Caso você queira apresentar os erros
encontrados, aí você deve ler o próximo item `EnsureErrorInformation()`.

### EnsuresErrorInformation()

Este método é basicamente um atalho para `EnsuresUsageInformation()`, porém com
a garantia que os erros serão primeiramente apresentados, antes da informação **`USAGE!`**.

E a impressão dos erros também só ocorre se o `Deletage` de Log de erros for
configurado, assim como `OnPrint Delegate`:

Você pode fazer isso usando **API Fluente**:

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

### Args opcional

Caso você não queira que a opção `<args>` apareça na informação de `USAGE:`, você pode
definir a propriedade `ShowArgs` como `false`.

Por padrão é assim:
```
USAGE:
    $ cmd [<options>|<flags>] [<args>]
```

Você pode desabilitar usando __API Fluente:__
```csharp
cmdApp.ShowArgs(false);
```

Ou em sua classe __Program:__
```csharp
class Program : CommandLineWrapper
{
    Program(string[] args) : base("cmd", "My Command Line App")
    {
        ShowArgs = false;
    }
}
```

No fim, ficará assim:
```
USAGE:
    $ cmd [<options>|<flags>]
```

## Só uma ideia

Em todos os exemplos que encontrei (só alguns na verdade), vi que o foco era a execução
de comandos. No meu caso, queria focar na exibição de ajuda e avaliação dos parâmetros.
O que a aplicação deve fazer, acredito que isso fique a cargo do projetista do próprio
programa.

Mas em alguns casos, é útil ter esse auxílio a execução de comandos. Então, segue uma
ideia de como podemos ter o melhor dos dois mundos, ou só a análise da linha de comando
e auxílio a exibição de ajdua, ou unir isso a um fluxo padrão para execução de comandos.

Abaixo só alguns rascunhos:


#### 1. Definimos nossos comandos
```csharp
class CommandLineCommands : CommandsWrapper
{
    CommandLineCommands(CommandLineWrapper program) : base(program) { }

    [CommandWrapper("Print hello world message")]
    void Hello(
        [ParamWrapper("The first name to print")]
        string name,
        
        [ParamWrapper("The second name to print", required: false)]
        string secondName)
    {
        if(!string.IsNullOrEmpry(secondName))
        {
            name = $"{secondName}, youName";
        }

        Say($"Hello {name}. Welcome to the {Program.Name} World!");
    }
}
```

#### 2. Definimos nosso programa
```csharp
class CommandLineProgram : CommandLineWrapper
{
    override string[] Synopsis { get; set; } = {
        "This is a my command line app to exemplify the use of this",
        "exceptional library. In the next few lines you'll see how",
        "easy it is to write a command line program."
    };

    CommandLineProgram() : base("cmd", "My Command Line App")
    {
        Version = "1.0.0-alpha-67890";
        Copyright = "(c) 1991-2018 E5R Development Team. All rights reserved.";
    }
}
```

#### 3. E enfim, definimos nosso programa que executa comandos
```csharp
class Program
    : CommandLineFullWrapper<CommandLineProgram, CommandLineCommands>
{
    static void Main(string[] args) => Launch(args);
}
```

### Ao executar

```powershell
$ program hello Erlimar
```
_@output:_
```
Hello Erlimar. Welcome to the World!
```

```powershell
$ program hello Erlimar "Silva Campos"
```
_@output:_
```
Hello Silva Campos, Erlimar. Welcome to the World!
```

### Um problema

Um problema é padronizar como isso irá coabitar com os parâmetros nomeados.
Porque um comando é em tese, não uma __option__:
```
$ cmd --option="Value of option"
```

E também não uma __flag__:

```
$ cmd --the-flag|-f
```

seria um __command__:
```
$ cmd command
```

Mas isso é o mesmo que um __named param__:
```
$ cmd named-param
```

E precisamos lembrar que o impacto são em dois lugares: Na exibição da ajuda,
e na execução do programa.

Então como diferenciamos um comando de um parâmetro nomeado nesses dois momentos?

#### Na ajuda

Na exibição da ajuda o que fazemos é exibir tudo, o mais completo possível:

```
cmd - My Command Line App

USAGE:
    $ cmd [<options>|<flags>] <Source> [<Target>|<args>]
    $ cmd [<options>|<flags>] <Command> [<CommandArgs>]

ARGS:
    <Source*>                   The source file path
    <Target>                    The target file path

COMMANDS:
    Hello                       Print hello world message
        Args:
        - name                  The first name to print
        - secondName            The second name to print

OPTIONS:
    -a|--action <Action>        The app action
    -k|--key <key>              Lorem ipsum lorem ipsum lorem ipsum
    -v|--verbosity=<verbosity>  Inform a verbosity level.
        Accepted values:
        - DEBUG                 ...
        - VERBOSE               ...
        - INFORMATION           ...
        - INFO                  ...
        - WARNING               ...
        - ERROR                 ...
        - CRITICAL              ...

FLAGS:
    -v|--verbose                Print verbose messages

(*) Argument is required
```

Aqui nós mostramos tudo:

* Se existir __options__, nós mostramos todas na seção `OPTIONS:` e apresentamos `<options>` em `USAGE:`
* Se existir __flags__, nós mostramos todas na seção `FLAGS:` e apresentamos `<flags>` em `USAGE:`
* Se existir __named params__, nós mostramos todos na seção `ARGS:` e apresentamos `<NamedParam> [<args>]` em `USAGE:`
* Se existir __commands__, nós mostramos todos na seção `COMMANDS:` e apresentamos uma nova linha de `USAGE:` contendo:
`$ cmd [<options>|<flags>] <Command> [<CommandArgs>]`.

Então no fim o que temos são duas formas de usar, ou seja, duas seções `USAGE:`, onde o usuário vai escolher
como usar, ou chamando um comando específico, ou só chamando o programa (o que seria como chamar um comando padrão,
algo como `RootCommand`).

#### Na execução

E para a execução, a proposta é simples:

Processamos normalmente as __options__ e __flags__, mas ao encontrar um parâmetro que não seja option ou flag,
e exista um comando registrado com esse nome, **Pronto!**. Executamos no modo comando.

Como o primeiro argumento foi reconhecido como o __command__, tudo mais serão os argumentos do comando.

Caso o primeiro argumento não seja reconhecido como um __command__, estamos no modo padrão, então reconhecemos
os parâmetros nomeados se existirem, e executamos o __command__ __`<RootCommand>`__.

O __`<RootCommand>`__ poderia ser um método na classe de programa:

```csharp
class Program : CommandLineWrapper
{
    Program(string[] args) : base("cmd", "My Command Line App") { }

    override void RootCommand()
    {
        // TODO: Implements!
    }

    static void Main(string[] args) => Launch(args);
}
```
