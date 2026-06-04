// ====================================
// Título: AuthJsonContext.cs
// Descrição: Source generator para serialização dos DTOs de autenticação
// ====================================

using System.Text.Json.Serialization;
using Portfolio.Web.DTOs.Auth;

namespace Portfolio.Web.Json;

[JsonSerializable(typeof(LoginRequestDto))]
[JsonSerializable(typeof(LoginResponseDto))]
public partial class AuthJsonContext : JsonSerializerContext { }