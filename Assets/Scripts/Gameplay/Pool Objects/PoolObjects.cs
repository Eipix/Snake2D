using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PoolObjects : MonoBehaviour
{
    [SerializeField] private List<Body> _bodys = new List<Body>();

    public Body Take(Body prefab, Vector3 position, Quaternion rotation,Transform parent, Color color)
    {      
        if(TryTakeFirst(prefab.GetType(), out Body first))
        {
            _bodys.Remove(first);
            first.transform.SetParent(parent);
            first.transform.position = position;
            first.transform.rotation = rotation;
            first.GetComponent<SpriteRenderer>().color = color;

            first.gameObject.SetActive(true);
            return first;
        }
        else
        {
            var body = Instantiate(prefab.gameObject, position, rotation, parent);
            body.GetComponent<SpriteRenderer>().color = color;
            return body.GetComponent<Body>();
        }
    }

    public void Put(Body body)
    {
        _bodys.Add(body);
        body.transform.position = gameObject.transform.position;
        body.transform.SetParent(gameObject.transform);
        body.gameObject.SetActive(false);
    }

    public List<Body> GetBodysByType(Body bodyType)
    {
        var bodys = _bodys.Where(body => body.GetType() == bodyType.GetType()).ToList();
        return bodys;
    }

    private bool TryTakeFirst(Type bodyType, out Body bodyFirst)
    {
        var bodys = _bodys.Where(body => body.GetType() == bodyType).ToList();
        bodyFirst = bodys.Count > 0 ? bodys[0] : null;
        return bodys.Count > 0;
    }
}
