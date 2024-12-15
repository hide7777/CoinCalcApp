package jp.co.auctor.helloworld.test;

import static org.junit.Assert.*;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.List;

import org.jboss.logging.Logger;
import org.junit.After;
import org.junit.AfterClass;
import org.junit.Before;
import org.junit.BeforeClass;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.test.context.ContextConfiguration;
import org.springframework.test.context.TestExecutionListeners;
import org.springframework.test.context.junit4.SpringJUnit4ClassRunner;
import org.springframework.test.context.support.DependencyInjectionTestExecutionListener;
import org.springframework.test.context.support.DirtiesContextTestExecutionListener;

import com.github.springtestdbunit.DbUnitTestExecutionListener;
import com.github.springtestdbunit.TransactionDbUnitTestExecutionListener;
import com.github.springtestdbunit.annotation.DatabaseSetup;

import jp.co.auctor.helloworld.entity.Users;
import jp.co.auctor.helloworld.service.UsersService;

@RunWith(SpringJUnit4ClassRunner.class)
@ContextConfiguration(locations = "classpath:application-test-context.xml")
@TestExecutionListeners({ DependencyInjectionTestExecutionListener.class,
        DirtiesContextTestExecutionListener.class,
        TransactionDbUnitTestExecutionListener.class,
        DbUnitTestExecutionListener.class })
@DatabaseSetup("classpath:testdata/users.xml")
public class UserServiceTest {

    /**
     * ロガー。
     * */
    Logger logger = Logger.getLogger(UserServiceTest.class);

    @Autowired
    UsersService usersService;

    @BeforeClass
    public static void execBeforeClass() {
    }

    @Before
    public void execBefore() {
    }

    @Test
    public void userServiceFindUsersTest01() {
        logger.info("★★★★★UserService.findUsers　test 01 ★★★★★");
        logger.info("*** 全件検索　***");
        try {
            List<Users> user = usersService.findUsers();
            if (user != null) {
                for (int i = 0; i < user.size(); i++) {
                    logger.info(
                            "■" + "ID=" + user.get(i).getId().toString() + " Name=" + user.get(i).getName().toString()
                                    + " EMail=" + user.get(i).getEmail().toString());
                }
            } else {
                fail("usersテーブルから１件も取得出来ませんでした。");
            }

            //件数を確認
            assertEquals(6, user.size());

        } catch (Exception e) {
            logger.error(e.toString());
            fail(e.toString()); //テスト失敗
        }
    }

    @Test
    public void userServiceFindUsersTest02() {
        logger.info("★★★★★UserService.findUsers　test 02 ★★★★★");
        logger.info("*** NO.2を検索　***");
        try {
            Long id = (long) 2000;
            Users user = usersService.findById(id);
            if (user != null) {
                logger.info("ID: " + id + " は以下の通りです");
                logger.info("■" + "ID=" + user.getId().toString() + " Name=" + user.getName().toString()
                        + " EMail=" + user.getEmail().toString());
            } else {
                fail("指定したデータをusersテーブルから取得出来ませんでした。");
            }

            //件数を確認
            assertNotNull(user);

        } catch (Exception e) {
            logger.error(e.toString());
            fail(e.toString()); //テスト失敗
        }
    }

    @Test
    public void userServiceFindUsersTest03() {
        logger.info("★★★★★UserService.findUsers　test 03 ★★★★★");
        try {
            Integer lank = 1;
            String startDate = "2018/03/01 00:00:00";
            String endDate = "2018/09/30 00:00:00";

            SimpleDateFormat sdFormat = new SimpleDateFormat("yyyy/MM/dd hh:mm:ss");
            Date start = sdFormat.parse(startDate);
            Date end   = sdFormat.parse(endDate);

            List<Users> user = usersService.findUsers3(lank,start, end);
            if (user != null) {
                for (int i = 0; i < user.size(); i++) {
                    logger.info(
                            "■" + "ID=" + user.get(i).getId().toString() + " startdate=" + user.get(i).getStartdate().toString()
                                    + " lank=" + user.get(i).getLank().toString());
                }
            } else {
                fail("usersテーブルから１件も取得出来ませんでした。");
            }

            //NULLを確認
            assertNotNull(user);

        } catch (Exception e) {
            logger.error(e.toString());
            fail(e.toString()); //テスト失敗
        }
    }

    @After
    public void execAfter() {
    }

    @AfterClass
    public static void execAfterClass() {
    }

}
