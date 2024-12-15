package jp.co.auctor.helloworld.repository;


import java.util.Date;
import java.util.List;
import java.util.stream.Stream;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.JpaSpecificationExecutor;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import jp.co.auctor.helloworld.entity.Users;

@Repository
public interface UsersRepository extends JpaRepository<Users, Long>, JpaSpecificationExecutor<Users> {
    public List<Users> findByNameContainsOrderByIdAsc(String name);

    public List<Users> findByEmailAndName(String email, String name);

    public List<Users> findByLankAndStartdateBetweenOrderByIdAsc(Integer lank,Date start, Date end);

    @Query("select t from Users t")
    Stream<Users> streamAll();
}
