namespace Sunset.Negocio;

public static class Validacao
{
    // Confere o CPF: 11 dígitos, não pode ser tudo igual e os dois dígitos
    // verificadores precisam bater. Recebe só os dígitos.
    public static bool CpfValido(string cpf)
    {
        if (cpf.Length != 11 || !cpf.All(char.IsDigit)) return false;
        if (cpf.Distinct().Count() == 1) return false;

        return cpf[9] - '0' == CalcularDigito(cpf, 9)
            && cpf[10] - '0' == CalcularDigito(cpf, 10);
    }

    // Soma cada dígito multiplicado por um peso que vai diminuindo (10..2 ou 11..2).
    private static int CalcularDigito(string cpf, int quantidade)
    {
        int soma = 0;
        for (int i = 0; i < quantidade; i++)
        {
            soma += (cpf[i] - '0') * (quantidade + 1 - i);
        }
        int resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }
}
