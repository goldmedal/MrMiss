using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponDll;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Windows.Forms;

namespace WeaponDll.Tests
{
    [TestClass()]
    public class BulletTests
    {
        [TestMethod()]
        public void BulletTest()
        {
            
        }

        [TestMethod()]
        public void BulletTest1()
        {
            
        }

        [TestMethod()]
        public void MoveTest()
        {
            // arrange
            Bullet target = new Bullet();
            target.BulletPic = new PictureBox();
            target.BulletPic.Location = new Point(1, 1);

            Point expected = new Point(125, 125);

            // actual
            target.Move(2, 2);
            Point actual = target.BulletPic.Location;
   
            // assert
            Assert.AreEqual(actual, expected);

        }
    }
}
