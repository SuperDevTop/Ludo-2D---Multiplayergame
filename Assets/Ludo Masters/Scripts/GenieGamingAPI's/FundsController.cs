using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FundsController : MonoBehaviour
{
    public void GotoFunds()
    {
        LinkOpener.OpenLinkStatic(GenieAPILinks.GenieAccountLogin);
    }
}
