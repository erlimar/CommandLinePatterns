```
%{PROGRAM} - %{DESCRIPTION}

${BRIEFING}

USAGE:

    $ %{PROGRAM} [option|flag] command [args]

COMMANDS:

    %{COMMAND}                  ${COMMAND_DESCRIPTION}
    ...

OPTIONS:
    -k|--key=[value]            Lorem ipsum lorem ipsum lorem ipsum
    -v|--verbosity=[value]      Inform a verbosity level.
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
    base a opção "--key", ou sua forma curta "-k". Se queremos atribuir o valor "Valor" a essa
    opção:
    
    --key=Valor | -k=Valor
    --key Valor | -k Valor
    
    Se precisar usar espaços:
    
    --key="Outro valor" | -k="Outro valor"
    --key "Outro valor" | -k "Outro valor"
    
  * As opções podem ter seus valores informados de forma livre, ou ter opções pré-definidas, como
    no caso da opção "--verbosity". Nesse caso temos duas coisas a dizer:
    
    1. Caso um valor diferente do esperado seja informado, um erro deve ser emitido pelo programa.
    2. As opções aceitas devem ser apresentadas na mensagem de ajuda.
    
  * Flags nunca tem valor. São simplesmente comutadores booleanos, estarão ligados ou desligados.
    No caso de flags, por padrão estão desligadas, e se informados estarão ligados. Tomando como
    exemplo, uma flag que habilita o modo de depuração, "--debug" ou sua forma curta "-d", temos:
    
    ```
    --debug | -d
    ```
    
    Nesse caso o modo de depuração "por padrão está desligado", mas como o informamos passa a ficar
    ligado.
    
    E se quiséssemos ao contrário? Por padrão ligado e o usuário pudesse desligá-lo.
    
    ```
    --no-debug | -n
    ```
    
    Não precisamos de um mecanismo que habilite FLAG's, tipo "-/d-" ou "-/d+" ou "--debug=+" "-d=-".
    Isso parece bem interessante a princípio, e uma implementação desse padrão não seria algo tão
    difícil. Mas pra quê complicar?
    
    O objetivo de FLAG's, são simplesmente chaves que receberão valores "TRUE" ou "FALSE", ligado ou
    desligado, não existe outra opção além disso. Então no caso de você deixar o usuário escolher
    sua forma, então basta informar qual o seu estado inicial, e possibilitá-lo mudar se for sua
    preferência.
    
    Então voltando ao caso "--debug".
    
    Se o modo está "habilitado" por padrão, então a única coisa que o usuário pode faser é "desabilitar".
    Então um "--no-debug" é o suficiente.
    
    Se o modo está "desabilitado" por padrão, logo a única coisa que o usuário pode fazer é "habilitar".
    Então um "--debug" é o suficiente.
    
    Veja que não há a necessidade de permitir ao usuário informar seu valor "BOOLEANO" com "+/-", "0/1",
    "S/N", "Y/N" e etc.
    
    Usamos a própria linguagem pra dizer isso. "Não-Depurar" ou "Depurar" é o que importa.
    
    No máximo podemos assumir um padrão de prefixação. Se a flag é "DEBUG" então as opções ao usuário
    seriam:
    
    ```
    --DEBUG e --NO-DEBUG, um prefixo "NO" é a negação.
    ```
    
    Mas isso fica a seu critério.
    
  * Quando usamos a forma curta dos parâmetros (ex: "-d" é a forma curta de "--debug") só teremos um
    caractere pra designar cada parâmetro, assim, podemos acumular vários parãmetros em uma única
	palavra.
	
	Ex: Suponha a seguinte configuração de parâmetros

  ```
    -d|--debug
    -h|--help
    -v|--verbosity
  ```
		
	No exemplo acima, se informarmos "-dhv" estamos informando "--debug", "--help" e "--verbosity" ao
	mesmo tempo.
	
	Até aí tudo bem, mas se considerarmos que um parâmetro pode ser uma "flat" ou uma "option", e
	que "flags" não tem valores, mas "options" tem valores, e que os valores podem ser informados
	tanto assim (--verbosity NORMAL, -v NORMAL), quanto assim (--verbosity=NORMAL, -v=NORMAL). Aí
	adicionamos uma pequena complexidade ao assumto.
	
	Como poderíamos informar flags e options com essa notação curta acumulada?
	Talvez não fique tão óbvio, mas seguindo o exemplo acima, poderia ser assim:
	
	"-dhv NORMAL"
	
	Isso corresponderia a:
	 - Flag "--debug" ligada
	 - Flag "--help" ligada
	 - Option "--verbosity" com valor "NORMAL"
	 
	 O que seria o mesmo se informássemos "-vhd" ou "-hvd". Ou seja, o que for "flag" simplesmente
	 será ligado, o que for "option" buscará o próximo item na fila como valor de sua opção.
	 
	 Quando disse que "talvez não fique tão óbvio", com esse exemplo, você me diga: Até que é meio
	 óbvio. Mas e se eu te der esse exemplo:
	 
	 Ex: Suponha a seguinte configuração de parâmetros
   
   ```
    -d|--debug-host
    -h|--help
    -v|--verbosity
   ```
		
	Nesse exemplo agora temos "--debug-host" como uma "option" que espera um endereço host de um
	servidor de depuração, uma flag "--help" e "--verbosity" como uma outra "option" .
	
	Então podemos informar assim:
	
	"-dhv 127.0.0.1 NORMAL"
	
	E isso corresponderia a:
	 - Option "--debug" com valor "127.0.0.1"
	 - Flag "--help" ligada
	 - Option "--verbosity" com valor "NORMAL"
	 
	 Que por sua vez também poderia ser informado de várias outras formas:
	 
	 - "-dvh 127.0.0.1 NORMAL"
	 - "-hdv 127.0.0.1 NORMAL"
	 - "-hvd NORMAL 127.0.0.1"
	 - "-vdh NORMAL 127.0.0.1"
	 - "-vhd NORMAL 127.0.0.1"
	 
	 Veja que basicamente o que muda é a ordem dos próximos parâmetros. E isso sim é o que pode
	 causar certa confusão, não ficando tão óbvia sua leitura.
	 
	 Mas enfim, não se pode ter tudo.
	 
	 Mas o bom é que podemos estabelecer sempre a seguinte regra:
	 
	 1. Sempre que uma "flag" for encontrada nos parâmetros, ela será ligada
	 2. Sempre que uma "option" for encontrada nos parâmetros, o próximo parâmetro será capturado
	    como valor dessa opção.
	 3. Sempre que uma "option" for informada, e não temos um próximo parâmetro para estabelecer
	    seu valor, aí temos um ERRO.
	 4. Sempre que um parâmetro for informado "aparentemente" como uma definição de "flag/option"
	    (ex: "-xyz") mas que tenha sido capturado erradamente como valor de uma "option" anterior,
		o efeito colateral será que tais parâmetros não serão reconhecidos como "flag/option".
		
	Na regra 4 acima, temos o relato de um problema, mas esse é minimizado se considerarmos que
	os valores de opções são previamente conhecidos e devem ter seu conteúdo validado pelo
	programa, esse ERRO será relatado e o usuário terá a chance de corrigir seu erro ao informar os
	parâmetros.
	
	E caso isso não ocorra, é um problema do programa, e como disse anteriormente "Mas enfim, não
	se pode ter tudo". Alguma coisa tinha que deixar na responsabilidade do programador. kkk.
	
