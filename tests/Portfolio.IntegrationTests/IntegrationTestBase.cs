// ====================================
// Título: IntegrationTestBase.cs
// Descrição: Classe base que compartilha a mesma factory/container
//            entre todos os testes de uma mesma classe
// ====================================

namespace Portfolio.IntegrationTests;

public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly HttpClient Client;

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Client = factory.CreateClient();
    }
}