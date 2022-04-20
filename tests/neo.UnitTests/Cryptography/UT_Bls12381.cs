using BLS12381Extend;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Neo.UnitTests.Cryptography
{
    [TestClass]
    public class UT_Bls12381
    {
        private byte[] g1 = "17F1D3A73197D7942695638C4FA9AC0FC3688C4F9774B905A14E3A3F171BAC586C55E83FF97A1AEFFB3AF00ADB22C6BB08B3F481E3AAA0F1A09E30ED741D8AE4FCF5E095D5D00AF600DB18CB2C04B3EDD03CC744A2888AE40CAA232946C5E7E1".ToLower().HexToBytes();
        private byte[] g2 = "13E02B6052719F607DACD3A088274F65596BD0D09920B61AB5DA61BBDC7F5049334CF11213945D57E5AC7D055D042B7E024AA2B2F08F0A91260805272DC51051C6E47AD4FA403B02B4510B647AE3D1770BAC0326A805BBEFD48056C8C121BDB80606C4A02EA734CC32ACD2B02BC28B99CB3E287E85A763AF267492AB572E99AB3F370D275CEC1DA1AAA9075FF05F79BE0CE5D527727D6E118CC9CDC6DA2E351AADFD9BAA8CBDD3A76D429A695160D12C923AC9CC3BACA289E193548608B82801".ToLower().HexToBytes();
        private byte[] gt = "C5851FA033E47219382577FD762BD397F9CD6BC96F54CEC81406D466733EF6CE80378481273411A625D8C63F8A44F31395699D2EB03163D27D7E79F782A4689D92EA398D24299B9CAA0731E1A21C80F466B0BCBD32076CA1780436BAAFA43C0841B61609DB61E2590D963EB2F4B61627459CBDA0105BE5C8A8ED4D9CD90BDB0BC5AAFD57BF9EF88C5E7A779E92B7D612355FE1B08851C85F6563098F3A6EA0342CD62AE0A62631DB0B999A7DA95A6FFC10C289EBF5552FA189886F923A70231778878271298F58938575AB11865BF643DF9F27ECF5AA8331F69DC98AE1D773FAB0994CA6A676E1641F8F38588CA79F1712EF2ACA110A2A676BF1A32AB5B9110D6E059D69D01244A4A55B1A2277011DC02955736CDECEE06639C3DD9F1EA7F50579C662B0A1880AD30483FC355D6AC55A0D291FA8A634C8D0C70737DAC23054CDF00A5080F77FC2F0AE2ED7E2A65D240956511B7976062E9F13FE184923C8D1E2F41B563C9F459E4CC1E3D3B9535EE8A32000A7211E120A82CC9AC5418361AF15B13A99248C65957CB986A81C7238EB73BC34744749D756528B4A50EA0219A48B6DCE860CF8D3A304AA6E68FB874AA61826CF20B91BE783BB4539A792AC77522AA046F0949FE50EFCF7586078F3CD5871F645F9821B06C17C67E5DB9FAA47F80357E63461A5DB78806E8A99439AECD71C6637991A9A59AAB144EE42082FF6A0C9FADF05B6E39B158EC23FF14A0DBA860CB1FF526AA0F20FE86C901A7248CA94761485B0033E188375E2E4CE40DDAF67F5FCA526E5D2966D9A42221F86499F7E19".ToLower().HexToBytes();

        [TestMethod]
        public void TestPointAdd()
        {
            byte[] value = Bls12381Extend.PointAdd(gt, gt);
            value.ToHexString().Should().Be("402497A074770B61022A9C47CAC7C3202E6BC37B0577D7A7DB16D75D4A8A829003264445170DF1EA6FED0A71A6A6CE080C195CE5484059A1907EE92B845515880513D50E9BB6B799AFBB6F0E7CD5EC73BD6E60AB8F9CFC9A5B56CBEA51C2471230D180AA5C5C1C9E8273109E38E3088CF274AC2557DE5B83869921074B0D86671A0F9C29F2EFF98A88B217413CF9951749AB1A0F087AEA423569973D6F333372735317F718DB4FB02BF10CB4EBE3D561348A84806E346F638D78D738C327FE09702AEFC2B366D61D3E51FEC4CC8728AE74DC841ABCEE6712F6D0CD32C1AFB5DA72D2F4D1A79228A743064756A83E550D252D52AE85391AB8F14E8C9F9BD214AAFB9848EBDD5B576200427EF3C6B46D498828052D497F10485B837019832A5506F29F57D95DD02BAE23B16A95B39C27E390BFE5AF8AD17A7C1F9A8BB38569C9FD0EDE65205D3809A15BE712F8017A4B08EF5B5F8ABE6ED60481BAD5062DA41843D7B79D6069E80BA3039C3AF5EEF9754B9010E7CA2468BEC3B7771280657A8F02073CF3AFD337C067497E6949BEED59B4608D6A02DABD266CFDA559AAE8D62F4FEA2D902FA62FE82A7C476767A8B10E10121D407439C3AC8C4C8DFAB2D85E0DB9F5FB07310C4940DDB05C3AC7192C1BE3CA7E52D0AA54CEBC8F6A3E24D34C0E045E156A4E6D0D3278FD158AEC34502B10F6D282D85D04C57358CB6CEA2D83C0FA8F5F4A3CEDD6653527CA7497BC9D910F75C066008B058E464FDD785371042E9B2E07F9650D141CAD293BB61861960326A89B437021BEB21857A231F127E8C116".ToLower());
        }

        [TestMethod]
        public void TestPointMul()
        {
            byte[] value1 = Bls12381Extend.PointMul(gt, 3);
            value1.ToHexString().Should().Be("AF587CA8F5F0760B888E4EC66748623D1B13986547B5DE2042CE89233C2C837BBB080329A505285C948D8BB4C88A5914AE216A64DB582BA084178DB4364A7B3843EF7FB9056A90526D27DC9E252B38600FC54CE7A5EC15771C555816EDC26D0166E75D3315789C846387C8C234B1C98B50BAF233B7312AA317AE56A2BFA170B1BC43F0E046B4A1F45CB7AACEA7D0AE0320496E3960D4DBC2039027BA8CFE1CA120EA98FA94CF25034CCB5E74033F447B837A78B03AFFD44B87F865F3D713000DE78AB5629DBDE8D0C8B7A28941717BE3EBD23924CF16897144BB69730F68BFF5A109FC2E6D563B15E2EB883CF4BECD11EDCBC2EC4402C93249389ED612FA0396ED6EADCBE4D703667D46B0150EE3A5158CAEF956791E4F527E1312C8402F8509ACF7001DC0DC311549D76398C247E79B737B614D0A6663F7DBDB314E5B51EACE14457EE7B1F3F54D5987C16C8F89D1022D91D4649F5F6A204047077E5791A654CD2277506CD0AF77F9EA789278B364C115EC07BA14390A9C22D59AA9C97A9C09CE025B5E14443F3C4E4CD602EF34105FADD82837FC5CE60A461E6EA11B13AE67B82E3366A2B2D1BBE78B2579173B3C0C5AED88B2949403060C3E065782BCB742C55C559E75E373293D80DEC54120773D80144B21B353EAD58DC8427E5B9CBD0C1431F1A74CAF5F57C4B55B89810029CDD24EF4A029797CED68FF882492A8F55F31FE52217356366983E35D2A5640E818CC8BDDF9274DE0100E624A6BF03E2B9FF22F8DDA09A46B50B1B305CB6ADFA2AE775FEBBCE4A2B507CD2390DB968CEB13".ToLower());
            byte[] value2 = Bls12381Extend.PointMul(gt, -3);
            value2.ToHexString().Should().Be("AF587CA8F5F0760B888E4EC66748623D1B13986547B5DE2042CE89233C2C837BBB080329A505285C948D8BB4C88A5914AE216A64DB582BA084178DB4364A7B3843EF7FB9056A90526D27DC9E252B38600FC54CE7A5EC15771C555816EDC26D0166E75D3315789C846387C8C234B1C98B50BAF233B7312AA317AE56A2BFA170B1BC43F0E046B4A1F45CB7AACEA7D0AE0320496E3960D4DBC2039027BA8CFE1CA120EA98FA94CF25034CCB5E74033F447B837A78B03AFFD44B87F865F3D713000DE78AB5629DBDE8D0C8B7A28941717BE3EBD23924CF16897144BB69730F68BFF5A109FC2E6D563B15E2EB883CF4BECD11EDCBC2EC4402C93249389ED612FA0396ED6EADCBE4D703667D46B0150EE3A5158CAEF956791E4F527E1312C8402F8509FFB2FEE23F23CDA4B628F0183CB8C482B07A4FA9966CCD6FE33653A529FA8C95C267CD5B04B425FD405FBECC5A882F177E192B9B60A09499BFB84C33A76E05CA56D339A6340281EFC5270C610C9812A3C1C04389A16E11AF7711E58F20976410DDA7A4A1EBBBBF7DB1B37DAE0FCB9BBF761D88BFA4754A5C79F416526938C9FC1E7E18DD13F5498FB25A5AC0D2D6C40D51BD764D6B6BFBB3F3C14D5A7C43F4DB5E995B582BEFBC3D8292A62D432B002757980022035431750C1E3DBB8E75430D97790E58B3509F623B4AF8277DFF825151A7BC557759B4795613FCCEF2A28104A6AEF8214351E5E11603220F94D11801DF1E2206D8B21EA9F19D09460EC1807F31C6231C972EC5160E5F7F281A6CD4B55F4D6086D1046643CDC2EF5D53851506".ToLower());
        }

        [TestMethod]
        public void TestPointPairing()
        {
            byte[] value = Bls12381Extend.PointPairing(g1, g2);
            value.ToHexString().Should().Be("C5851FA033E47219382577FD762BD397F9CD6BC96F54CEC81406D466733EF6CE80378481273411A625D8C63F8A44F31395699D2EB03163D27D7E79F782A4689D92EA398D24299B9CAA0731E1A21C80F466B0BCBD32076CA1780436BAAFA43C0841B61609DB61E2590D963EB2F4B61627459CBDA0105BE5C8A8ED4D9CD90BDB0BC5AAFD57BF9EF88C5E7A779E92B7D612355FE1B08851C85F6563098F3A6EA0342CD62AE0A62631DB0B999A7DA95A6FFC10C289EBF5552FA189886F923A70231778878271298F58938575AB11865BF643DF9F27ECF5AA8331F69DC98AE1D773FAB0994CA6A676E1641F8F38588CA79F1712EF2ACA110A2A676BF1A32AB5B9110D6E059D69D01244A4A55B1A2277011DC02955736CDECEE06639C3DD9F1EA7F50579C662B0A1880AD30483FC355D6AC55A0D291FA8A634C8D0C70737DAC23054CDF00A5080F77FC2F0AE2ED7E2A65D240956511B7976062E9F13FE184923C8D1E2F41B563C9F459E4CC1E3D3B9535EE8A32000A7211E120A82CC9AC5418361AF15B13A99248C65957CB986A81C7238EB73BC34744749D756528B4A50EA0219A48B6DCE860CF8D3A304AA6E68FB874AA61826CF20B91BE783BB4539A792AC77522AA046F0949FE50EFCF7586078F3CD5871F645F9821B06C17C67E5DB9FAA47F80357E63461A5DB78806E8A99439AECD71C6637991A9A59AAB144EE42082FF6A0C9FADF05B6E39B158EC23FF14A0DBA860CB1FF526AA0F20FE86C901A7248CA94761485B0033E188375E2E4CE40DDAF67F5FCA526E5D2966D9A42221F86499F7E19".ToLower());
        }
    }
}
