package rb.org.domain;

import java.util.HashMap;

/**
 * Created by bogaa on 6/4/2017.
 */
public class IdentityHolder {

    HashMap<String, Identity> identityHashMap;

    public IdentityHolder()
    {
        this.identityHashMap = new HashMap<String, Identity>();
    }

    /**
     * Add a new Identity (Connection).
     * @param correlationId
     * @param identity collection of data required to indentify a user.
     * @return identity of old user
     */
    public Identity putIdentity(String correlationId, Identity identity)
    {
        Identity foundIdentity = null;
        foundIdentity = getIdentity(correlationId);

        if(foundIdentity != null)
        {
            this.identityHashMap.remove(correlationId); //remove old connection
        }

        this.identityHashMap.put(correlationId, identity);

        return foundIdentity;
    }

    public void removeIdentity(String correlationId){
        this.identityHashMap.remove(correlationId);
    }

    /**
     * Get the identity of a user using the alternative keyvalue than that of the sender.
     * @param correlationId
     * @return identity of user
     */
    public Identity getIdentity(String correlationId)
    {
        return this.identityHashMap.get(correlationId);
    }
}
