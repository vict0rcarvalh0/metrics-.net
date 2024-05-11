# Coleta de métricas em .NET
## Introdução
No mundo do desenvolvimento de software, entender o desempenho e o comportamento de seus aplicativos é crucial. As métricas fornecem uma visão detalhada do funcionamento interno dos seus sistemas, permitindo uma otimização eficiente e uma resposta rápida a quaisquer problemas que possam surgir. Nesse sentido, as etapas a seguir abordam a criação de métricas personalizadas, utilizando as APIs System.Diagnostics.Metrics do .NET.

## Pré-requisitos
- <a href="https://dotnet.microsoft.com/pt-br/download/dotnet/8.0">.NET 8</a>

- IDE de sua preferência e com compatibilidade com o seu sistema operacional(<a href="https://www.jetbrains.com/pt-br/rider/">JetBrains Rider</a> ou <a href="https://visualstudio.microsoft.com/pt-br/downloads/">Microsoft Visual Studio</a>)
- dotnet.counters
    - Pode ser instalado depois de ja possuir o SDK do .NET instalado por meio do comando:
        ```bash
            dotnet tool update -g dotnet-counters
        ```

## Tecnologias utilizadas
- .NET SDK 8
    - Plataforma de desenvolvimento de software que oferece um conjunto de ferramentas e bibliotecas para criar aplicativos modernos e escaláveis na plataforma .NET. Ele inclui recursos como compiladores, bibliotecas, runtime e ferramentas de linha de comando para desenvolver e implantar aplicativos .NET em diversos ambientes.
- Rider IDE
    - IDE desenvolvida pela JetBrains, projetada especificamente para desenvolvedores .NET.
- Thunder Client
    - Extensão para o Visual Studio Code que fornece um ambiente de teste de API simples e eficiente.
- Terminal ZSH
    - Shell de linha de comando avançado para sistemas operacionais Unix-like, como macOS e Linux.

## Criando uma Métrica Personalizada
Vamos começar com a criação de uma métrica personalizada para acompanhar o número de chapéus vendidos em uma loja fictícia, a HatCo.

### 1. Criação de uma Console Application
Para isso, foi criado um novo aplicativo de console via IDE, que também pode ser criado pelo comando `dotnet new console`.

<p align="center">
    <img src="/assets/1.png" width="80%">
</p>

A saída foi a seguinte, com o template de Console Application vazio:

<p align="center">
    <img src="/assets/2.png" width="80%">
</p>

### 2. Configuração do projeto
Depois disso, a etapa que foi seguida foi a de adicionar a referência necessária ao pacote NuGet System.Diagnostics.DiagnosticSource à aplicação por meio do gerenciador de pacotes NuGet na IDE, que também pode ser adicionado por meio do comando `dotnet add package System.Diagnostics.DiagnosticSource`.

<p align="center">
    <img src="/assets/3.png" width="80%">
</p>

### 3. Implementação do código
Em seguida, o código a seguir foi implementado à classe Program:

```c#
using System;
using System.Diagnostics.Metrics;
using System.Threading;

class Program
{
    static Meter s_meter = new Meter("HatCo.Store");
    static Counter<int> s_hatsSold = s_meter.CreateCounter<int>("hatco.store.hats_sold");

    static void Main(string[] args)
    {
        Console.WriteLine("Press any key to exit");
        while(!Console.KeyAvailable)
        {
            Thread.Sleep(1000);
            s_hatsSold.Add(4);
        }
    }
}
```

Na IDE, o resultado ficou assim:
<p align="center">
    <img src="/assets/4.png" width="80%">
</p>

O código implementado cria uma métrica para acompanhar o número de chapéus vendidos na loja HatCo e a cada 1 segundo adiciona 4 chapéus vendidos pela loja.

### 4. Execução do aplicativo e acompanhamento das métricas
Depois disso, a aplicação foi executada com o comando `dotnet run` e em outro terminal o comando `dotnet-counters monitor -n Metrics --counters HatCo.Store` foi executado para acompanhar as métricas de vendas de chapéu da loja HatCo.Store.

<p align="center">
    <img src="/assets/6.png" width="80%">
</p>

A conclusão que foi observada é que em dois segundos rodando a aplicação e acompanhando o medidor, 8 chapéus foram vendidos, indicando um comportamento de sucesso em relação ao código implementado.

## Obtendo um Medidor por meio da injeção de dependência
A injeção de dependência (DI) é uma prática fundamental no desenvolvimento de aplicativos modernos, especialmente em frameworks como ASP.NET Core. Nesta etapa, exploraremos como obter um medidor usando a injeção de dependência, uma abordagem mais flexível e recomendada em ambientes que utilizam DI, como o ASP.NET Core.

### 1. Criação de uma Web Application
Para isso, foi criado um novo aplicativo web via IDE, que também pode ser criado pelo comando `dotnet new web`.

<p align="center">
    <img src="/assets/pt2.9.png" width="80%">
</p>

A saída foi a seguinte, com o template de Web Application:

<p align="center">
    <img src="/assets/pt2.10.png" width="80%">
</p>

### 2. Configuração do projeto
Depois disso, a etapa que foi seguida foi a de adicionar a referência necessária ao pacote NuGet System.Diagnostics.DiagnosticSource à aplicação por meio do gerenciador de pacotes NuGet na IDE, que também pode ser adicionado por meio do comando `dotnet add package System.Diagnostics.DiagnosticSource`. Exatamente da mesma forma que na seção "Criando uma Métrica Personalizada".

### 3. Implementação do código
#### **3.1. Definindo o Tipo para Armazenar os Instrumentos:**

Nesta etapa, foi criada uma classe no projeto chamada HatCoMetrics, que representa as métricas da loja HatCo. Esta classe recebe o IMeterFactory no construtor para criar o medidor de forma dinâmica e possui o método HatsSold, que recebe um parâmetro da quantidade vendida e adiciona ao contador das métricas. O seguinte código foi implementado:

```c#
public class HatCoMetrics
{
    private readonly Counter<int> _hatsSold;

    public HatCoMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("HatCo.Store");
        _hatsSold = meter.CreateCounter<int>("hatco.store.hats_sold");
    }

    public void HatsSold(int quantity)
    {
        _hatsSold.Add(quantity);
    }
}
```

Na IDE, o resultado ficou assim:
<p align="center">
    <img src="/assets/pt2.7.png" width="80%">
</p>


#### **3.2. Definindo o Tipo para Armazenar os Instrumentos:**

Nesta etapa, foi criada uma pasta na raiz da aplicação, nomeada por Models, dentro dela, uma classe SaleModel foi criada com a finalidade de representar o tipo que será armazenado na instrumentação da métrica, que no caso, carrega a quantidade de chapéus vendida. O código implementado foi o seguinte:

```c#
public class SaleModel
{
    public int QuantitySold { get; set; }

}
```

Na IDE, o resultado ficou assim:
<p align="center">
    <img src="/assets/pt2.8.png" width="80%">
</p>


#### **3.3. Registrando o Tipo com Container de DI e utilizando o tipo registrado em um Endpoint:**

Por fim, a classe HatCoMetrics foi registrada com o container de injeção de dependência. Isso permite que o ASP.NET Core gerencie a criação e o ciclo de vida dessa classe conforme necessário. Com isso feito, é possível utilizar a classe HatCoMetrics nos controladores, APIs ou outros tipos que foram criados pelo DI. Na implementação, ao completar uma venda, o método HatsSold é chamado para registrar o número de chapéus vendidos. O código implementado foi o seguinte:

```c#
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1;
using WebApplication1.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

# Registrando o tipo com container de DI
builder.Services.AddSingleton<HatCoMetrics>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

# Utilizando o tipo registrado em um controller
app.MapPost("/complete-sale", ([FromBody] SaleModel model, HatCoMetrics metrics) =>
{
    metrics.HatsSold(model.QuantitySold);
    })
    .WithName("CreateCompleteSale")
    .WithOpenApi();

app.Run();
```

O resultado na IDE ficou assim:

<p align="center">
    <img src="/assets/pt2.5.png" width="80%">
</p>

### 4. Execução do aplicativo e acompanhamento das métricas
Depois disso, a aplicação foi executada com o comando `dotnet run` e em outro terminal o comando `dotnet-counters monitor -n WebApplication1 --counters HatCo.Store` foi executado para acompanhar as métricas de vendas de chapéu da loja HatCo.Store.

<p align="center">
    <img src="/assets/pt2.6.png" width="80%">
</p>

Para testar o funcionamento, o endopoint foi executado passando como parâmetro 3 chapéus vendidos por meio do Thunder Client, que é uma aplicação que permite testar APIs web. O resultado foi o seguinte:

<p align="center">
    <img src="/assets/pt2.1.png" width="80%">
</p>
<p align="center">
    <img src="/assets/pt2.2.png" width="80%">
</p>

Outro teste foi feito eceutando o endpoint com o parâmetro de 50 chapéus vendidos, e o resultado foi o seguinte:

<p align="center">
    <img src="/assets/pt2.3.png" width="80%">
</p>
<p align="center">
    <img src="/assets/pt2.4.png" width="80%">
</p>

## Conclusão:

Ao adotar métricas importantes, é possível obter uma compreensão mais profunda do desempenho e do comportamento dos aplicativos, resultando em melhores decisões e otimizações. A obtenção de medidores por meio da injeção de dependência destaca-se como uma abordagem flexível e eficiente, especialmente em ambientes como o ASP.NET Core. Ao integrar práticas recomendadas de injeção de dependência e métricas, os desenvolvedores podem garantir a criação e o gerenciamento adequados dos medidores, promovendo a manutenção e a escalabilidade dos aplicativos. Concluindo, a contínua exploração das possibilidades oferecidas por essas APIs é fundamental para aprimorar ainda mais os aplicativos e proporcionar uma experiência excepcional aos usuários.

## Conceitos aprendidos
### Dependency Injection(Injeção de dependência)

A prática de Injeção de Dependência (DI), que é fundamental para a construção de aplicativos robustos e escaláveis no ecossistema .NET. Utilizando DI, é possível obter instâncias de objetos de forma flexível, facilitando a gestão das dependências e promovendo a modularidade e testabilidade do código.

### Dotnet counters

Ferramenta poderosa para análise ad hoc de métricas em aplicativos .NET. Com o dotnet-counters, é possível monitorar e analisar diversas métricas em tempo real, fornecendo insights valiosos sobre o desempenho e comportamento dos nossos aplicativos.

### Instrumentação de métricas

É nítida importância da instrumentação de métricas em aplicativos para acompanhar e entender o desempenho e comportamento dos sistemas. Utilizando as APIs System.Diagnostics.Metrics, é possível criar métricas personalizadas, permitindo uma análise detalhada do funcionamento interno dos nossos aplicativos e possibilitando uma melhor tomada de decisão e otimização.