using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CardWarWEB
{
    public class ObjectCopy
    {
        /// <summary>
        /// 深度克隆 ,相当于new,也就是在内存中重新创建  不受原来属性的改变而改变 
        /// </summary>
        /// <returns></returns>
        public object DeepCopy(object obj)
        {
            //该方法要求  如果当前类的某个属性也是一个对象，则要求该属性对象要序列化
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
            stream.Position = 0;
            return formatter.Deserialize(stream) as object;
        }

    }
}
