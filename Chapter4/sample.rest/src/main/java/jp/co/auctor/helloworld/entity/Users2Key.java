package jp.co.auctor.helloworld.entity;

import java.io.Serializable;

import javax.persistence.Embeddable;

@Embeddable
public class Users2Key implements Serializable {
    private Long id;
    private Long num;

    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public Long getNum() {
        return num;
    }

    public void setNum(Long num) {
        this.num = num;
    }

    @Override
    public int hashCode() {
        final int prime = 31;
        int result = 1;
        result = prime * result + ((num == null) ? 0 : num.hashCode());
        result = prime * result + ((id == null) ? 0 : id.hashCode());
        return result;
    }

    private boolean checkKey(Object src,Object dest) {
        if (src == null) {
            if (dest != null) {
                return false;
            }
        } else if (!src.equals(dest)) {
            return false;
        }
        return true;
    }

    @Override
    public boolean equals(final Object obj) {
        if (this == obj) {
            return true;
        }
        if (obj == null) {
            return false;
        }
        if (getClass() != obj.getClass()) {
            return false;
        }

        Users2Key other = (Users2Key) obj;
        return (checkKey(num,other.num) && checkKey(id,other.id));
    }

}
