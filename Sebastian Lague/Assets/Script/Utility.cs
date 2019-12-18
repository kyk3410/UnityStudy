using System.Collections;
using System.Collections.Generic;


// 내부 메소드에 쉽게 접근할 수 있도록, public static class로 지정한다, 그리고 MonoBehaviour를 상속할 필요도, UnityEngine을 사용할 필요도 없다.
public static class Utility 
{
    // 셔플 메소드가 public static이 되며, 셔플된 배열을 반환하게 한다, 하지만 배열이 어떤 타입이 될지 모르니 특정한 타입을 가리키는 것이 아닌
    // 제네릭 타입 T를 사용한다. T오브젝트의 배열을 쓰자, 입력으로 셔플안된 배열을 주려고 한다 T타입의 배열로 array라는 이름으로 준다.
    // seed값을 주어 언제나 같은 규칙으로 섞을 수 있게 해준다.
    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        System.Random prng = new System.Random(seed);
        // 배열의 모든 원소를 거쳐 for루프 한다, -1 하는 이유는 마지막 루프는 생략해도 되기 때문이다.
        for(int i=0; i<array.Length -1; i++)
        {
            // 랜덤인덱스,              여기에 최솟값과 최댓값을 줄수있는데 알고리즘에 따르면 앞에 i가 오고 뒤에 end로 배열의 길이가 온다 
            int randomIndex = prng.Next(i, array.Length);
            // i번째 원소를 랜덤 원소와 교체한다, 그래서 덮어쓰기를 방지하기 위해 먼저 둘 중 하나를 저장해둬야 한다.
            // 임시저장할 T tempItem 선언 그리고 array[randomIndex]로, 무작위로 선택한 랜덤 아이템을 할당해준다.
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem; // tempItem 할당
        }

        return array;
    }
}
