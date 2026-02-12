using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClickerGame
{
    public class BigNumber : IComparable<BigNumber>
    {
        private const int BASE = 1000;
        private List<int> digits; // младший разряд по индексу 0

        #region Конструкторы
        public BigNumber()
        {
            digits = new List<int> { 0 };
        }

        public BigNumber(int value)
        {
            digits = new List<int>();
            if (value == 0)
                digits.Add(0);
            else
            {
                while (value > 0)
                {
                    digits.Add(value % BASE);
                    value /= BASE;
                }
            }
        }

        public BigNumber(long value)
        {
            digits = new List<int>();
            if (value == 0)
                digits.Add(0);
            else
            {
                while (value > 0)
                {
                    digits.Add((int)(value % BASE));
                    value /= BASE;
                }
            }
        }

        public BigNumber(BigNumber other)
        {
            digits = new List<int>(other.digits);
        }

        public BigNumber(string str)
        {
            digits = new List<int>();
            str = str.TrimStart('0');
            if (string.IsNullOrEmpty(str)) str = "0";
            int len = str.Length;
            for (int i = len; i > 0; i -= 3)
            {
                int start = Math.Max(0, i - 3);
                int partLen = i - start;
                string part = str.Substring(start, partLen);
                digits.Add(int.Parse(part));
            }
            RemoveLeadingZeros();
        }
        #endregion

        #region Приватные методы
        private void RemoveLeadingZeros()
        {
            while (digits.Count > 1 && digits.Last() == 0)
                digits.RemoveAt(digits.Count - 1);
        }

        private void Normalize()
        {
            for (int i = 0; i < digits.Count; i++)
            {
                if (digits[i] >= BASE)
                {
                    int carry = digits[i] / BASE;
                    digits[i] %= BASE;
                    if (i + 1 < digits.Count)
                        digits[i + 1] += carry;
                    else
                        digits.Add(carry);
                }
            }
            RemoveLeadingZeros();
        }
        #endregion

        #region Арифметические операции
        public BigNumber Add(BigNumber other)
        {
            BigNumber result = new BigNumber(this);
            int maxLen = Math.Max(result.digits.Count, other.digits.Count);
            for (int i = 0; i < maxLen; i++)
            {
                if (i >= result.digits.Count)
                    result.digits.Add(0);
                if (i < other.digits.Count)
                    result.digits[i] += other.digits[i];
            }
            result.Normalize();
            return result;
        }

        public BigNumber Subtract(BigNumber other)
        {
            // Не допускаем отрицательного результата
            if (this.CompareTo(other) < 0)
                return new BigNumber(0);

            BigNumber result = new BigNumber(this);
            for (int i = 0; i < other.digits.Count; i++)
            {
                result.digits[i] -= other.digits[i];
                if (result.digits[i] < 0)
                {
                    // Заём у следующего разряда
                    int j = i + 1;
                    while (j < result.digits.Count && result.digits[j] == 0)
                    {
                        result.digits[j] = BASE - 1;
                        j++;
                    }
                    if (j < result.digits.Count)
                    {
                        result.digits[j]--;
                    }
                    result.digits[i] += BASE;
                }
            }
            result.RemoveLeadingZeros();
            return result;
        }

        // Умножение на целое число
        public BigNumber Multiply(int multiplier)
        {
            if (multiplier == 0)
                return new BigNumber(0);

            BigNumber result = new BigNumber(this);
            for (int i = 0; i < result.digits.Count; i++)
                result.digits[i] *= multiplier;
            result.Normalize();
            return result;
        }

        // Умножение на вещественное число (с округлением до целого)
        public BigNumber Multiply(double multiplier)
        {
            if (multiplier == 0)
                return new BigNumber(0);

            // Сначала умножаем на 1000, затем делим на 1000 (сохраняем 3 знака)
            long factor = (long)Math.Round(multiplier * 1000);
            BigNumber temp = this.Multiply((int)factor);
            return temp.Divide(1000);
        }

        // Деление на целое число (целочисленное деление)
        public BigNumber Divide(int divisor)
        {
            if (divisor == 0)
                throw new DivideByZeroException();

            BigNumber result = new BigNumber();
            result.digits.Clear();
            long remainder = 0;

            for (int i = digits.Count - 1; i >= 0; i--)
            {
                remainder = remainder * BASE + digits[i];
                result.digits.Insert(0, (int)(remainder / divisor));
                remainder %= divisor;
            }
            result.RemoveLeadingZeros();
            return result;
        }

        // Умножение на другой BigNumber (для полноты)
        public BigNumber Multiply(BigNumber other)
        {
            BigNumber result = new BigNumber(0);
            for (int i = 0; i < other.digits.Count; i++)
            {
                BigNumber temp = new BigNumber(this);
                temp = temp.Multiply(other.digits[i]);
                for (int j = 0; j < i; j++)
                    temp.digits.Insert(0, 0);
                result = result.Add(temp);
            }
            return result;
        }
        #endregion

        #region Сравнение
        public int CompareTo(BigNumber other)
        {
            if (other == null) return 1;
            if (digits.Count != other.digits.Count)
                return digits.Count.CompareTo(other.digits.Count);
            for (int i = digits.Count - 1; i >= 0; i--)
            {
                if (digits[i] != other.digits[i])
                    return digits[i].CompareTo(other.digits[i]);
            }
            return 0;
        }

        public static bool operator >(BigNumber a, BigNumber b) => a.CompareTo(b) > 0;
        public static bool operator <(BigNumber a, BigNumber b) => a.CompareTo(b) < 0;
        public static bool operator >=(BigNumber a, BigNumber b) => a.CompareTo(b) >= 0;
        public static bool operator <=(BigNumber a, BigNumber b) => a.CompareTo(b) <= 0;
        #endregion

        #region Преобразование в строку
        public override string ToString()
        {
            if (digits.Count == 0) return "0";
            StringBuilder sb = new StringBuilder(digits.Last().ToString());
            for (int i = digits.Count - 2; i >= 0; i--)
                sb.Append(digits[i].ToString("D3"));
            return sb.ToString();
        }
        #endregion
    }
}