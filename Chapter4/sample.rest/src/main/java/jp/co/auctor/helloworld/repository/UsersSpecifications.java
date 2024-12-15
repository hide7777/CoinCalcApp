package jp.co.auctor.helloworld.repository;

import org.springframework.data.jpa.domain.Specification;
import org.springframework.util.StringUtils;

import jp.co.auctor.helloworld.entity.Users;

public class UsersSpecifications {
    /**
     * idを含むデータを検索する。
     */
    public static Specification<Users> idContains(Long id) {
        return StringUtils.isEmpty(id) ? null : (root, query, cb) -> {
                return cb.equal(root.get("id"), id);
        };
    }

    /**
     * ユーザー名を含むデータを検索する。
     */
    public static Specification<Users> nameContains(String name) {
        return StringUtils.isEmpty(name) ? null : (root, query, cb) -> {
            return cb.like(root.get("name"), "%" + name + "%");
        };
    }

    /**
     * emailを含むデータを検索する。
     */
    public static Specification<Users> emailContains(String email) {
        return StringUtils.isEmpty(email) ? null : (root, query, cb) -> {
            return cb.like(root.get("email"), "%" + email + "%");
        };
    }
}
