using api;
using app.Services;
using auth;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace test.Fixtures
{
    public class AuthTest : TestBed<Base>
    {
        ClaimsPrincipal Usuario;
        AuthService authService;

        public AuthTest(ITestOutputHelper testOutputHelper, Base fixture) : base(testOutputHelper, fixture)
        {
            authService = fixture.GetService<AuthService>(testOutputHelper)!;
        }

        public (string Token, ClaimsPrincipal Usuario) AutenticarUsuario(AppController controller, AuthUserModel<Permissao>? usuario = null, List<Permissao>? permissoes = null)
        {
            if (usuario == null)
            {
                usuario = new AuthUserModel<Permissao>
                {
                    Id = 1,
                    Name = "test",
                    Permissions = Enum.GetValues<Permissao>().ToList(),
                };
            }
            if (permissoes != null)
            {
                usuario.Permissions = permissoes;
            }

            var (token, _) = authService.GenerateToken(usuario);

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            Usuario = new ClaimsPrincipal(new ClaimsIdentity(jwt.Claims));
            controller.AppUsuario = Usuario;
            return (token, Usuario);
        }
    }
}
