```
%{PROGRAM} - %{DESCRIPTION}

${BRIEFING}

USAGE:

    $ %{PROGRAM} [option|flag] command [args]

COMMANDS:

    %{COMMAND}                  ${COMMAND_DESCRIPTION}
    ...

OPTIONS:
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
    ...

FLAGS:
    -f|--flag                   Lorem ipsum lorem ipsum lorem ipsum
    -o|--other-flag             Lorem ipsum lorem ipsum lorem ipsum
```

# NOTAS:

* Uma opção é um parâmetro de chave=valor que pode ser informado da seguinte forma (tomando como
base a opção `--key`, ou sua forma curta `-k`. Se queremos atribuir o valor **"Valor"** a essa
opção:

```
--key=Valor | -k=Valor
--key Valor | -k Valor
```
    
Se precisar usar espaços:

```
--key="Outro valor" | -k="Outro valor"
--key "Outro valor" | -k "Outro valor"
```
    
* As opções podem ter seus valores informados de forma livre, ou ter opções pré-definidas, como
no caso da opção `--verbosity`. Nesse caso temos duas coisas a dizer:

  1. Caso um valor diferente do esperado seja informado, um erro deve ser emitido pelo programa.
  2. As opções aceitas devem ser apresentadas na mensagem de ajuda.

* Flags nunca tem valor. São simplesmente comutadores booleanos, estarão ligados ou desligados.
No caso de flags, por padrão estão desligadas, e se informados estarão ligados. Tomando como
exemplo, uma flag que habilita o modo de depuração, `--debug` ou sua forma curta `-d`, temos:
    
```
--debug | -d
```
    
Nesse caso o modo de depuração **"por padrão está desligado"**, mas como o informamos, passa a
estar ligado.
    
E se quiséssemos o contrário? Por padrão ligado e o usuário pudesse desligá-lo.
    
```
--no-debug | -n
```
    
Não precisamos de um mecanismo que habilite **flag's**, tipo `-/d-` ou `-/d+` ou `--debug=+` `-d=-`.
Isso parece bem interessante a princípio, e uma implementação desse padrão não seria algo tão
difícil. Mas pra quê?
    
O objetivo de uma **flag**, é simplesmente informar se está (**"TRUE"**) ou não (**"FALSE"**) 
habilitada, ligado ou desligado, não existe outra opção além disso.

Então no caso de você deixar o usuário escolher sua forma, basta informar qual o seu estado inicial,
e possibilitá-lo mudar se for de sua preferência.
    
Então voltando ao caso `--debug`.
    
Se o modo está **"habilitado"** por padrão, a única coisa que o usuário pode faser é **"desabilitar"**.
Então um `--no-debug` é o suficiente.
    
Se o modo está **"desabilitado"** por padrão, logo a única coisa que o usuário pode fazer é **"habilitar"**.
Então um `--debug` é o suficiente.
    
Veja que não há a necessidade de permitir ao usuário informar seu valor **"BOOLEANO"** com `+/-`, `0/1`,
`S/N`, `Y/N` e etc. Usamos o próprio sentido da palavra (nome da flag) pra dizer isso.
**"Não-Depurar"** ou **"Depurar"** já diz tudo.
    
No máximo podemos assumir um padrão de prefixação. Se a flag é **"DEBUG"** então as opções ao usuário
seriam:
    
```
--DEBUG e --NO-DEBUG, um prefixo "NO" é a negação.
```
    
Mas isso fica a seu critério, e vamos combinar, não é muito útil. Um **BOOLEAN** é muito claro para ter
uma configuração implícita com prefixos. Deixar você ser explícito definindo todas suas flags é muito
mais prático.

* Quando usamos a forma curta dos parâmetros (ex: `-d` é a forma curta de `--debug`) só teremos um
caractere pra designar cada parâmetro, assim, podemos acumular vários parâmetros em uma única palavra.
	
Ex: Suponha a seguinte configuração de parâmetros

  ```
    -d|--debug
    -h|--help
    -v|--verbosity
  ```
		
No exemplo acima, se informarmos `-dhv` estamos informando `--debug`, `--help` e `--verbosity` ao
mesmo tempo.
	
Até aí tudo bem, mas se considerarmos que um parâmetro pode ser uma **"flag"** ou uma **"option"**,
e que **flag's** não tem valores, mas **option's** tem valores; e que os valores podem ser informados
tanto assim (`--verbosity NORMAL`, `-v NORMAL`), quanto assim (`--verbosity=NORMAL`, `-v=NORMAL`).
Aí adicionamos uma pequena complexidade ao assumto.
	
Como poderíamos informar **flag's** e **option's** com essa notação curta acumulada?
Talvez não fique tão óbvio, mas seguindo o exemplo acima, poderia ser assim:
	
```
-dhv NORMAL
```
	
Isso corresponderia a:

- Flag `--debug` ligada
- Flag `--help` ligada
- Option `--verbosity` com valor **NORMAL**

O que seria o mesmo se informássemos `-vhd` ou `-hvd`. Ou seja, o que for **flag** simplesmente
será ligado, o que for **option** buscará o próximo item na fila como valor de sua opção.

Quando disse que **"talvez não fique tão óbvio"**, e vendo esse exemplo você me diga:
_"Até que é meio óbvio"_.
 
Mas e se eu te der este outro exemplo?

Ex: Suponha a seguinte configuração de parâmetros:
   
```
-d|--debug-host
-h|--help
-v|--verbosity
```
Nesse exemplo agora temos `--debug-host` como uma **option** que espera um endereço host de um
servidor de depuração, uma flag `--help`, e `--verbosity` como uma outra **option**.

Então podemos informar assim:

```
-dhv 127.0.0.1 NORMAL
```

E isso corresponderia a:

- Option `--debug` com valor **127.0.0.1**
- Flag `--help` ligada
- Option `--verbosity` com valor **NORMAL**

Que por sua vez também poderia ser informado de várias outras formas:

- `-dvh 127.0.0.1 NORMAL`
- `-hdv 127.0.0.1 NORMAL`
- `-hvd NORMAL 127.0.0.1`
- `-vdh NORMAL 127.0.0.1`
- `-vhd NORMAL 127.0.0.1`

Veja que basicamente o que muda é a ordem dos próximos parâmetros. E isso sim é o que pode
causar certa confusão, não ficando tão óbvia sua leitura.

**Temos um outro pequeno ponto que talvez cause confusão.**

Ex: Suponha a seguinte configuração de parâmetros:

```
-d|--debug
-e|--environment
-b|--build
-u|--unknow
-g|--global
```

No exemplo acima, podemos supor que todos são _flag's_, mas também poderiam ser _option's_ sem
problemas. Então se informarmos isso aqui na linha de comando:

```
-debug
```

O que queremos dizer é que todas as _flags_ foram atribuídas, ou seja, `--debug`, `--environment`,
`--build`, `--unknow`, e `--global`.

Mas se ao invés disso tivéssemos informado isso:

```
--debug
```

O que queremos dizer é que somente a flag `--debug` foi atribuída.

Como regra é simples dizer que se o parâmetro iniciar com `-` trata-se de um ou mais nomes curtos
de _flag's_ ou _option's_, e se o parâmetro iniciar com `--` trata-se de um nome completo, de _flag_ 
ou _option_.

Mas visualmente, `-debug` é só um pouco diferente de `--debug`, e por se tratar de um assunto relacionado
a UI (User Interface), o foco é o usuário. Como ele (usuário) é que interage com nosso programa, isso pode
deixá-lo um tanto quanto confuso, pois um simples esquecimento ou erro de digitação pode fazer o programa
se comportar de forma totalmente diferente, e isso não é bom para o usuário.

Mas enfim, não se pode ter tudo. O bom é que nós podemos estabelecer algumas regras pra deixar tudo menos
difícil:

1. Quando iniciar com `-` (único traço), cada caractere é um nome curto, e para cada um:
   - Sempre que uma **flag** for encontrada nos parâmetros, ela será ligada;
   - Sempre que uma **option** for encontrada nos parâmetros, o próximo parâmetro será capturado
     como valor dessa opção;
   - Sempre que uma **option** for encontrada nos parâmetros, e não temos um próximo parâmetro
     para estabelecer seu valor, aí temos um ERRO;
2. Quando iniciar com `--` (dois traços), o termo inteiro é o nome do item:
   - Se for uma **flag**, ela será ligada;
   - Se for uma **option**, e tem um `=` no meio, quer dizer que o que está após o `=` é o
     valor da opção;
   - Se for uma **option**, e não tem um `=`, o próximo parâmetro será capturado como valor dessa opção;
   - Se for uma **option**, e não tem um `=`, e não temos um próximo parâmetro para estabelecer seu valor,
     aí temos um ERRO;
   - Se for uma **option**, e tem um `=`, mas não tem nada após o `=`, aí temos um ERRO.
	 
Podemos seguir essas regras e considerar que nosso programa está pronto para lidar com essas situações,
mas também podemos abrir mão de algumas coisas para não gerar essas confusões ao usuário, e deixar só umas
regras mais simples:

1. Começou com `-` (único traço), sempre serão **flag's**, nunca **option's**;
2. Começou com `--` (dois traços), pode ser **flag** ou **option**;
3. Sempre que um parâmetro de **flag** (iniciando com `-`) tiver mais de um caractere, e esses caracteres
   formam o nome longo de uma **flag** ou **option**. Aí temos um ERRO.
   
Com a regra **1**, e **2**, eliminamos a primeira confusão do próximo parâmetro ser o valor de uma das
options e a ordem causar confusão, visto que não haverão options quando iniciado com `-`, portanto um
valor nunca é esperado.

Com a regra **3**, eliminamos a segunda confusão do conflito de nomes e evitar a confusão do usuário
com `-debug` e `--debug`.

Ainda temos muito a dizer sobre esse simples assunto chamado **parâmetros de linha de comando**,
mas vamos falando por aí em outros textos.
