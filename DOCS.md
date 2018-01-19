# Documenta��o `(RASCUNHO)`

Segue umas pequenas notas do que se pretende com esse projeto.

### CommandLineApp m�nima

```csharp
var cmdApp = new CommandLineApp("cmd", "My Command Line App");

cmdApp.PrintUsage();
```
_@output:_
```
cmd - My Command Line App

USAGE:
    $ cmd [<options>|<flags>] [<args>]
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
    $ cmd [<options>|<flags>] [<args>]
```

### CommandLineApp com direitos autorais

```csharp
var cmdApp = new CommandLineApp("cmd", "My Command Line App");

cmdApp.Copyright("(c) 1991-%{YEAR} E5R Development Team. All rights reserved.");

cmdApp.PrintUsage();
```
_@output:_
```
cmd - My Command Line App
Copyright (c) 1991-2018 E5R Development Team. All rights reserved.

USAGE:
    $ cmd [<options>|<flags>] [<args>]
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
    $ cmd [<options>|<flags>] [<args>]
```

### CommandLineApp mais completo

```csharp
var cmdApp = new CommandLineApp("cmd", "My Command Line App")

    .Version("1.0.0-alpha-67890");
    .Copyright("(c) 1991-%{YEAR} E5R Development Team. All rights reserved.");
    .Synopsis(new string[] {
        "This is a my command line app to exemplify the use of this",
        "exceptional library. In the next few lines you'll see how",
        "easy it is to write a command line program." });

cmdApp.PrintUsage();
```
_@output:_
```
cmd - My Command Line App [version 1.0.0-alpha-67890]
Copyright (c) 1991-2018 E5R Development Team. All rights reserved.

USAGE:
    $ cmd [<options>|<flags>] [<args>]
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
    $ cmd [<options>|<flags>] [<args>]
```

_@output:_
```
cmd - My Command Line App [version 1.0.0-alpha-67890]
Copyright (c) 1991-2018 E5R Development Team. All rights reserved.

This is a my command line app to exemplify the use of this
exceptional library. In the next few lines you'll see how
easy it is to write a command line program.

USAGE:
    $ cmd [<options>|<flags>] [<args>]
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

Assert.True(options.HasValue("Action"));
Assert.Equal("My Action Value", options.ValueOf("Action"));

Assert.False(options.HasValue("NotFoundAction"));
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

Assert.True(options.HasValue("Action"));
Assert.Equal("My Action Value", options.ValueOf("Action"));
Assert.False(options.HasValue("NotFoundAction"));

Assert.True(options.Defined("Verbose"));
Assert.False(options.Defined("NotFoundFlag"));
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