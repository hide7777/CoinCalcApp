package jp.co.auctor.helloworld.service;


import java.util.Date;
import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Sort;
import org.springframework.data.jpa.domain.Specifications;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import jp.co.auctor.helloworld.entity.Users;
import jp.co.auctor.helloworld.repository.UsersRepository;
import jp.co.auctor.helloworld.repository.UsersSpecifications;

@Service
@Transactional
public class UsersService {
    @Autowired
    UsersRepository usersRepository;

    /**
     *　すべてのユーザーを検索する。
     */
    public List<Users> findUsers() {
        return usersRepository.findAll(new Sort(Sort.Direction.ASC, "id"));
    }

    /**
    *　会員番号か氏名かE-MAILのいずれか、指定された値を使ってユーザーを検索する。
    */
    public List<Users> findUsers(Long id, String name, String email) {
        return usersRepository.findAll(Specifications
                .where(UsersSpecifications.idContains(id))
                .and(UsersSpecifications.nameContains(name))
                .and(UsersSpecifications.emailContains(email)), new Sort(Sort.Direction.ASC, "id"));
    }

    /**
     *　select * from Users where email = $email and name = $name 。
     */
    public List<Users> findUsers2(String name, String email) {
        return usersRepository.findByEmailAndName(email, name);
    }

    /**
     *　select * from Users where lank = $lank and startdate between $start and $end 。
     */
    public List<Users> findUsers3(Integer lank,Date start, Date end) {
        return usersRepository.findByLankAndStartdateBetweenOrderByIdAsc(lank,start, end);
    }

    /**
     *　氏名を使ってユーザーを検索する。
     */
    public List<Users> findByName(String name) {
        return usersRepository.findByNameContainsOrderByIdAsc(name);
    }

    /**
     *　会員番号を使ってユーザーを検索する。
     */
    public Users findById(Long id) {
        return usersRepository.findOne(id);
    }

    /**
     *　渡されてユーザーをテーブルにINSERTする。
     */
    @Transactional
    public Users save(Users users) {
        return usersRepository.saveAndFlush(users);
    }

    /**
     *　渡されてユーザーでテーブルをUPDATEする。
     */
    @Transactional
    public Users update(Users users) {
        return usersRepository.saveAndFlush(users);
    }

    /**
     * 会員番号を使ってユーザーをテーブルからDELETEする。
     */
    @Transactional
    public void delete(Long id) {
        usersRepository.delete(id);
    }
}