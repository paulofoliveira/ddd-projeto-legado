# ddd-projeto-legado
Praticando DDD com o curso "Domain-Driven Design: Working with Legacy Projects" by Vladimir Khorikov.

Algumas lições importantes aprendidas no curso:

Existem várias definições sobre o que é um código ou projeto legado, e que pode ser definido de várias formas. A forma que adotei como definição para um "código legado" é um código que há esforço significativo para mantê-lo. Reforça a ideia do PDD (Pain Driven Development) no processo de desenvolvimento de software.

Entropia do Software define a medida de desordem de um sistema sendo uma medida de disponibilidade da energia. É uma grandeza física que está relacionada com a segunda lei da Termodinâmica e tende a aumentar naturalmente no universo. https://todamateria.com.br/entropia.

Então, temos como conclusão que a entropia do software sempre cresce, o código tende a deteriorar e tornar o "Big ball of Mud" ou "grande bola de lama" com muito código spaguetti e duplicações.

Questões sobre refatoração, quando refatorar e em que situações. Uma frase interessante do Eric Evans sobre:
"Keep everthing well-design is the enemy of good design". Refatoração é essencial, mas precisa ser estratégico. Fato!

ACL - Camada anti-corrupção que é usada para "separar" o modelo de domínio do código legado. Temos uma "Bubble" dentro de um projeto legado. Este projeto precisa de comunicação com o modelo atual de alguma forma, e é ai que entra a camada anti-corrupção.

A "bolha" não conhece nada sobre o legado. Conversa com a ACL que usa linguagem ubíqua do DDD e executa toda a tradução para o legacy mode.

Há muitos outros aprendizados durante o curso, sobre conceitos do DDD (Objetos de valor, entidades e agregados), Validações de erros (Validation Erros vs Preconditions) e contratos de código (Code Contracts),

Tornando a camada bolha autônoma com storage separada e criando uma comunicação entre a "nova camada" e o modelo atual com uma ACL, agora em contexto próprio como um serviço Windows, por exemplo. Há necessidades neste caso de estratégias de sincronização de dados e melhor análise do fluxo e do tempo para atualização dos dados.

Envolvimento de ACL para comunicação com N domínios, por exemplo com a utilização de chamadas REST através de APIs e brokers de comunicação através da troca de mensagens usando modelo de "push" and "pull".
