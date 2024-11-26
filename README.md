# peleanobolicho
An assignment of a 3D game in unity

Visão Geral
"Peleia no Bolicho" é um jogo de ação em primeira pessoa com elementos de combate
corpo a corpo e armas brancas, ambientado em um típico bar sul-rio-grandense, conhecido
localmente como Bolicho. O jogo incorpora o estilo regional ao representar uma "peleia"
(briga) em um ambiente desafiador e divertido. Originalmente planejado como um roguelike
3D com geração procedural de dungeons, o projeto evoluiu para um jogo com mapa
estático e uma estrutura inspirada em conquistas de torre, devido a limitações no
desenvolvimento.
Características Principais
● Ambientação Regional: Inspirado na cultura do Rio Grande do Sul, o jogo acontece
em um Bolicho, com combates típicos em um ambiente rústico.
● Dificuldade Personalizável: O jogador pode escolher entre os níveis Fácil, Normal e
Difícil.
● Sistema de Combate: Armas brancas e golpes de artes marciais.
● Sistema de Stamina, que limita a corrida e dificulta ações em excesso.
● Sistema de partículas para representar golpes e sangue.
● Exploração e Recompensas: Itens de recuperação de HP.
● Armas brancas escondidas em salas de tesouro.
● Objetivo: Eliminar todos os inimigos de cada sala até alcançar e vencer a sala final.
● Recursos Sonoros: Sons de golpes, uso de armas, uso de itens, interações com
botões e notificações de dano.
● Cenas e Telas: Menu Inicial: Seleção de dificuldade e início do jogo.
End Game: Tela de game over, com opções para reiniciar o jogo, voltar ao menu ou
sair.
● Jogo: Combate no mapa principal.
Estrutura do Jogo
Cenas:
Menu:
Tela de Start: Opção para iniciar o jogo.
Tela de Difficulty: Seleção do nível de dificuldade.
Endofgame: Tela de fim de jogo exibida quando o jogador vence ou perde.
testSCENE: Representa o mapa estático do jogo, com salas de combate
interconectadas.
Sistemas Implementados
● Sistema de Stamina: Reduz a barra conforme o jogador corre, adicionando uma
camada de estratégia.
● Itens no Mapa: Objetos para recuperação de HP e armas brancas, distribuídos em
salas específicas.
● Sistema de Partículas: Golpes com armas brancas geram emissões de partículas
para maior imersão visual.
● Game Over: Tela exibida quando o jogador vence ou é derrotado.
Assets e Recursos
Apenas assets gratuitos foram utilizados no projeto. Sons e animações foram
adicionados para enriquecer a experiência.
Metodologia de Desenvolvimento
● Equipe: Giovanni Assonalio Cerbaro, Nicolas Comin Todero
● Início do Projeto: O desenvolvimento começou com um foco em geração procedural
de dungeons, mas dificuldades técnicas levaram à mudança para um mapa estático.
● Organização do Trabalho: Não houve estrutura formal de planejamento. As tarefas
foram realizadas de forma espontânea, conforme as preferências de cada
desenvolvedor.
● Foram usadas duas cenas paralelas (SampleScene e testSCENE) para evitar
conflitos de versão.
Versionamento e Repositório:
Utilização de GIT e GitHub para armazenamento e colaboração
