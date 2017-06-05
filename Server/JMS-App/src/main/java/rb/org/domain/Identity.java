package rb.org.domain;

import javax.jms.Destination;

/**
 * Created by bogaa on 6/4/2017.
 */
public class Identity {

    Destination destination;

    public Identity(Destination destination)
    {
        this.destination = destination;
    }

    public Destination getDestination()
    {
        return this.destination;
    }
}
