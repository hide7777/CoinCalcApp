package jp.co.auctor.helloworld.entity;

import java.io.Serializable;

import javax.persistence.Column;
import javax.persistence.EmbeddedId;
import javax.persistence.Entity;
import javax.persistence.Table;

@Entity
@Table(name = "users2")
//@IdClass(value = Users2Key.class)
public class Users2 implements Serializable {

    @EmbeddedId
    private Users2Key key = null;

    @Column(name = "name")
    private String name;

    @Column(name = "email")
    private String email;

    public Users2Key getKey() {
        return key;
    }

    public void setKey(Users2Key key) {
        this.key = key;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

}
