package jp.co.auctor.helloworld.test;

import static org.junit.Assert.*;

import java.io.FileNotFoundException;
import java.io.IOException;
import java.net.URISyntaxException;
import java.sql.SQLException;
import java.util.stream.Stream;

import javax.persistence.EntityManager;
import javax.persistence.PersistenceContext;

import org.jboss.logging.Logger;
import org.junit.After;
import org.junit.AfterClass;
import org.junit.Before;
import org.junit.BeforeClass;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.test.annotation.Rollback;
import org.springframework.test.context.ContextConfiguration;
import org.springframework.test.context.TestExecutionListeners;
import org.springframework.test.context.junit4.SpringJUnit4ClassRunner;
import org.springframework.test.context.support.DependencyInjectionTestExecutionListener;
import org.springframework.test.context.support.DirtiesContextTestExecutionListener;
import org.springframework.transaction.annotation.Transactional;

import com.github.springtestdbunit.DbUnitTestExecutionListener;
import com.github.springtestdbunit.TransactionDbUnitTestExecutionListener;
import com.github.springtestdbunit.annotation.DatabaseSetup;

import jp.co.auctor.helloworld.entity.Users;
import jp.co.auctor.helloworld.repository.UsersRepository;

@RunWith(SpringJUnit4ClassRunner.class)
@ContextConfiguration(locations = "classpath:application-test-context.xml")
@TestExecutionListeners({ DependencyInjectionTestExecutionListener.class,
        DirtiesContextTestExecutionListener.class,
        TransactionDbUnitTestExecutionListener.class,
        DbUnitTestExecutionListener.class })
@DatabaseSetup("classpath:testdata/users.xml")
public class UserRepositoryTest {

    Logger logger = Logger.getLogger(UserRepositoryTest.class);

    @PersistenceContext
    private EntityManager em;

    @Autowired
    UsersRepository usersRepository;

    @BeforeClass
    public static void execBeforeClass() {
    }

    @Before
    public void execBefore() throws SQLException, FileNotFoundException, IOException, URISyntaxException {
    }

    int size;

    @Test
    @Rollback(true)
    @Transactional
    public void userRepositoryTest01() throws Exception {
        logger.info("★★★★★UserRepository.findAll　test 01 ★★★★★");
        try {
            Long id = (long) 2000;
            logger.info("NO." + id + "のデータを削除");
            usersRepository.delete(id); //１行削除

            //List<Users> user = usersRepository.findAll(new Sort(Sort.Direction.ASC, "id"));
            Stream<Users> user = usersRepository.streamAll();

            if (user != null) {
                size = 0;
                user.forEach(u -> {
                	logger.info("■" + "ID=" + u.getId().toString() + " Name="
                            + u.toString()
                            + " EMail=" + u.getEmail().toString());
                    em.detach(u);
                    size++;
                });
            } else {
                fail("usersテーブルから１件も取得出来ませんでした。");
            }

            //テストデータから１件削除した後の件数を確認
            assertEquals(5, size);

        } catch (Exception e) {
            logger.error(e.toString());
            fail(e.toString()); //テスト失敗
        }
    }

    @After
    public void execAfter() throws FileNotFoundException, SQLException, URISyntaxException {
    }

    @AfterClass
    public static void execAfterClass() {
    }

}
