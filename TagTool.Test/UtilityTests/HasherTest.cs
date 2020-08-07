using TagTool.Data.Security;
using Xunit;

namespace TagTool.Test
{
    public class HasherTest
    {
      
        /* Test Cases */
        // 8, 16, 32, 64, 128

        [Fact]
        public void Hasher_ShouldNotReturnSameString()
        {
            // Given
            string input8 = "p6330euV";
            string input16 = "4pfnj9MemkUjrDIU";
            string input32 = "jERAsctrfnnGzxLPyPzWIOLrw6qfUr9z";
            string input64 = 
            "URt4mZK89Zx1K5UnODR7APFp4v4ONu9y2s7XnpVIBfo4qJoL7i8ZGZdar8Je12cB";
            string input128 = 
            "ZI2eXtuGCIyHUIIhuk5NAVhXOh0fNr3q4hK3rMENJNXyIsDw4yWSbWzHEym646ijZi"
            + "EACW0EddyqpTqAZ9AKindvMs7TWHBX7xGpoffbWVlCq0naTwDv8Bzkur95Et3b";

            // When
            var Hashed8 = Hasher.CalculateHash(input8);
            var Hashed16 = Hasher.CalculateHash(input16);
            var Hashed32 = Hasher.CalculateHash(input32);
            var Hashed64 = Hasher.CalculateHash(input64);
            var Hashed128 = Hasher.CalculateHash(input128);

            // Then - We Expect the result of hashing to be different than input
            Assert.NotEqual(Hashed8, input8);
            Assert.NotEqual(Hashed16, input16);
            Assert.NotEqual(Hashed32, input32);
            Assert.NotEqual(Hashed64, input64);
            Assert.NotEqual(Hashed128, input128);
        }

        [Fact]
        public void Hasher_CanValidate_8CharacterPassword()
        {
            // Given
            string input = "p6330euV";

            // When
            var Hashed = Hasher.CalculateHash(input);
            
            // Then
            Assert.True(Hasher.ValidateHash(Hashed, input));
        }

        [Fact]
        public void Hasher_CanValidate_16CharacterPassword()
        {
            // Given
            string input = "4pfnj9MemkUjrDIU";

            // When
            var Hashed = Hasher.CalculateHash(input);

            // Then
            Assert.True(Hasher.ValidateHash(Hashed, input));
        }

        [Fact]
        public void Hasher_CanValidate_32CharacterPassword()
        {
            // Given
            string input = "jERAsctrfnnGzxLPyPzWIOLrw6qfUr9z";

            // When
            var Hashed = Hasher.CalculateHash(input);

            // Then
            Assert.True(Hasher.ValidateHash(Hashed, input));
        }

        [Fact]
        public void Hasher_CanValidate_64CharacterPassword()
        {
            // Given
            string input = 
            "URt4mZK89Zx1K5UnODR7APFp4v4ONu9y2s7XnpVIBfo4qJoL7i8ZGZdar8Je12cB";

            // When
            var Hashed = Hasher.CalculateHash(input);

            // Then
            Assert.True(Hasher.ValidateHash(Hashed, input));
        }

        [Fact]
        public void Hasher_CanValidate_128CharacterPassword()
        {
            // Given
            string input = 
            "ZI2eXtuGCIyHUIIhuk5NAVhXOh0fNr3q4hK3rMENJNXyIsDw4yWSbWzHEym646ijZi"
            + "EACW0EddyqpTqAZ9AKindvMs7TWHBX7xGpoffbWVlCq0naTwDv8Bzkur95Et3b";

            // When
            var Hashed = Hasher.CalculateHash(input);

            // Then
            Assert.True(Hasher.ValidateHash(Hashed, input));
        }
    }
}